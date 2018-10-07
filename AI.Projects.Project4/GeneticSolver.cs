using System;
using System.Collections.Generic;
using System.Linq;
using AI.Projects.Shared.Events;
using AI.Projects.Shared.Interfaces;
using AI.Projects.Shared.Models;

namespace AI.Projects.Project4
{
    public class GeneticSolver : ISolver
    {
        public EventHandler<BestFoundEventArgs> NewBestTrip { get; set; }
        public EventHandler<EventArgs> DataCleared;

        /// <summary>
        /// A property that stores the starting point of the path
        /// </summary>
        public City Origin { get; set; }
        /// <summary>
        /// A property that stores a list of the points that can be visited
        /// </summary>
        public List<City> Destinations { get; set; }
        /// <summary>
        /// A property that stores the current best solution
        /// </summary>
        public Trip BestTrip { get; set; }
        /// <summary>
        /// A property that stores the current size of the population
        /// </summary>
        public int PopulationSize { get; set; }
        /// <summary>
        /// A property that stores the current population
        /// </summary>
        public List<Trip> Population { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public GeneticSolver()
        {
            Destinations = new List<City>();
            Population = new List<Trip>();
        }

        /// <summary>
        /// A method that parses a list of cities into properties used by the solver
        /// </summary>
        /// <param name="cities">The cities to be parsed</param>
        public void OrderData(List<City> cities)
        {
            // Clear any previous information
            PopulationSize = 10;
            Destinations.Clear();

            // Set the first city as the start and end
            Origin = cities[0];

            // Add the rest of the cities to the destinations list
            Destinations = cities.Where(c => c != Origin).ToList();
        }

        /// <summary>
        /// A method that runs the solver to get the result
        /// </summary>
        public void GetShortestPath()
        {
            GenerateInitialPopulation();
        }

        /// <summary>
        /// A method that randomly generates the first 10 trips
        /// </summary>
        private void GenerateInitialPopulation()
        {
            int nIndex;
            List<City> route;
            Random random = new Random();
            
            for (int x = 0; x < PopulationSize; x++)
            {
                route = Destinations;

                for(int i = 0; i < route.Count - 1; i++)
                {
                    nIndex = random.Next(route.Count);
                    City temp = route[i];
                    route[i] = route[nIndex];
                    route[nIndex] = temp;
                }

                var path = new Trip(Origin, route, true);
                Population.Add(path);
            }

            BestTrip = Population[0];
            GetBestTrip();
        }

        private void Genocide()
        {

        }

        private void GetBestTrip()
        {
            bool update = false;

            foreach (Trip canidate in Population.Skip(1))
            {
                if (canidate.Fitness > BestTrip.Fitness)
                {
                    BestTrip = canidate;
                    update = true;
                }
            }

            if (update)
                NewBestTrip.Invoke(this, new BestFoundEventArgs(BestTrip));
        }
    }
}
