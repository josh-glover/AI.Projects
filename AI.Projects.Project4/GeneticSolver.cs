using System;
using System.Collections.Generic;
using System.Linq;
using AI.Projects.Shared.Interfaces;
using AI.Projects.Shared.Models;

namespace AI.Projects.Project4
{
    public class GeneticSolver : ISolver
    {
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
            Random random = new Random();
            
            for (int x = 0; x < PopulationSize; x++)
            {
                List<City> route = new List<City>();

                for (int y = 0; y < Destinations.Count; y++)
                {
                    int index = random.Next(Destinations.Count);
                    bool added = false;

                    while(!added)
                    if (!route.Contains(Destinations[index]))
                    {
                        route.Add(Destinations[index]);
                        added = true;
                    }
                    else
                    {
                        index++;
                        if (index >= Destinations.Count)
                            index = 0;
                    }
                }

                var path = new Trip(Origin, route, true);
                Population.Add(path);
            }
        }

        private void Genocide()
        {

        }
    }
}
