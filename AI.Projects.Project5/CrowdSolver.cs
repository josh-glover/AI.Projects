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

namespace AI.Projects.Project5
{
    public class CrowdSolver : ISolver, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public EventHandler<TripAddedEventArgs> NewTripFound;

        public GeneticSolver GSolver { get; set; }
        public bool Running { get; set; }
        public City Origin { get; set; }
        public List<City> Destinations { get; set; }
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

            while (Running)
            {
                GSolver.GetShortestPath();

                while (GSolver.Running) { /* Wait for a solution */ }

                pathCount++;
                Trip newTrip = GSolver.BestTrip;
                newTrip.Name = $"Trip {pathCount}";
                NewTripFound?.Invoke(this, new TripAddedEventArgs(newTrip));
            }
        }

        public void OrderData(List<City> cities)
        {
            GSolver.OrderData(cities);
        }

        private void AggregateTrips()
        {
            Trip newTrip = new Trip(new List<City>());

            for (int x = 0; x < Destinations.Count + 2; x++)
            {
                City canidateCity;
                Dictionary<City, int> cityFrequencies = new Dictionary<City, int>();

                for (int y = 0; y < TripCollection.Count; y++)
                {
                    City city = TripCollection[y].Stops[x];

                    if (cityFrequencies.Keys.Contains(city))
                        cityFrequencies[city] += 1;
                    else
                        cityFrequencies.Add(city, 1);
                }

                // Get the most frequent, break ties from shortest distance

            }
        }

        protected virtual void NotifyOfPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
