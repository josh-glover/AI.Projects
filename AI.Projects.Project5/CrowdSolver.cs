using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using AI.Projects.Project4;
using AI.Projects.Shared.Events;
using AI.Projects.Shared.Interfaces;
using AI.Projects.Shared.Models;
using AI.Projects.Shared.Utilities;

namespace AI.Projects.Project5
{
    public class CrowdSolver : ISolver, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public EventHandler<BestFoundEventArgs> NewBestFound;
        public EventHandler<TripAddedEventArgs> NewTripFound;

        public GeneticSolver GSolver { get; set; }
        public bool Running { get; set; }
        public City Origin { get; set; }
        public List<City> Destinations { get; set; }
        public List<City> AllCities { get; set; }
        public Trip BestTrip { get; set; }
        public List<Trip> TripCollection { get; set; }

        public CrowdSolver()
        {
            GSolver = new GeneticSolver(99999);
            Destinations = new List<City>();
            TripCollection = new List<Trip>();
        }

        public void GetShortestPath()
        {

            Running = true;
            int pathCount = 0;
            BestTrip = new Trip(new List<City>());

            while (Running)
            {
                GSolver.GetShortestPath();

                while (GSolver.Running) { /* Wait for a solution */ }

                pathCount++;
                Trip newTrip = GSolver.BestTrip;
                newTrip.Name = $"Trip {pathCount}";
                TripCollection.Add(newTrip);
                NewTripFound?.Invoke(this, new TripAddedEventArgs(newTrip));

                AggregateTrips();
            }
        }

        public void OrderData(List<City> cities)
        {
            Origin = cities[0];
            Destinations = cities.Skip(1).ToList();
            AllCities = cities;
            GSolver.OrderData(cities);
        }

        private void AggregateTrips()
        {
            Trip newTrip = new Trip(new List<City>());
            City[] stops = new City[AllCities.Count + 1];

            for (int x = 0; x < Destinations.Count + 2; x++)
            {
                int highestFreq = 0;
                int[] cityFrequencies = new int[AllCities.Count];

                foreach (Trip trip in TripCollection)
                {
                    City city = trip.Stops[x];

                    cityFrequencies[city.Index - 1]++;

                    if (cityFrequencies[city.Index - 1] > highestFreq)
                        highestFreq = cityFrequencies[city.Index - 1];
                }

                List<City> pCities = new List<City>();

                for (int z = 0; z < cityFrequencies.Length; z++)
                {
                    City cityOption = AllCities.Find(c => c.Index == z + 1);

                    if (cityFrequencies[z] == highestFreq)
                        pCities.Add(cityOption);
                }


                //if (pCities.Count > 1)
                //{
                //    if (x == 0)
                //    {
                //        stops[x] = pCities[0];
                //        return;
                //    }

                //    City cCity = pCities[0];

                    for (int a = 0; a < pCities.Count; a++)
                    {
                        if(pCities[a] != stops[0] && stops.Contains(pCities[a]))
                            continue;
                        stops[x] = pCities[a];
                    }
                //}
                //else if (pCities.Count == 1)
                //{
                //    stops[x] = pCities[0];
                //}
            }

            newTrip.Stops = stops.ToList();
            BestTrip = newTrip;
            NewBestFound?.Invoke(this, new BestFoundEventArgs(0, BestTrip));
        }

        protected virtual void NotifyOfPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
