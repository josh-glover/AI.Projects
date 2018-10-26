using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
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
            GSolver.NewBestTrip += OnNewBestTrip;
            Running = true;

            while (Running)
            {
                GSolver.GetShortestPath();

                while (GSolver.Running) { /* Wait for a solution */ }

                TripCollection.Add(GSolver.BestTrip);
            }
        }

        public void OrderData(List<City> cities)
        {
            GSolver.OrderData(cities);
        }

        private void OnNewBestTrip(object sender, BestFoundEventArgs args)
        {

        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
