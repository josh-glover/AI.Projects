using System;
using System.Collections.Generic;
using System.Linq;
using AI.Projects.Project4;
using AI.Projects.Shared.Events;
using AI.Projects.Shared.Interfaces;
using AI.Projects.Shared.Models;

namespace AI.Projects.Project5
{
    public class CrowdSolver : ISolver
    {
        // Event Handlers
        public EventHandler<BestFoundEventArgs> NewBestFound;
        public EventHandler<TripAddedEventArgs> NewTripFound;

        /// <summary>
        /// A property that stores the genetic solver to generate solutions
        /// </summary>
        public GeneticSolver GSolver { get; set; }
        /// <summary>
        /// A property that indicates the status of the algorithm
        /// </summary>
        public bool Running { get; set; }
        /// <summary>
        /// A property that stores the starting point of the path
        /// </summary>
        public City Origin { get; set; }
        /// <summary>
        /// A property that stores a list of the points that can be visited
        /// </summary>
        public List<City> Destinations { get; set; }
        /// <summary>
        /// A property that stores the current best trip
        /// </summary>
        public Trip BestTrip { get; set; }
        /// <summary>
        /// A property that stores a matrix of the frequencies a city appears in a spot of a path
        /// </summary>
        public int[][] FrequencyMatrix { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public CrowdSolver()
        {
            GSolver = new GeneticSolver(49999);
            Destinations = new List<City>();
        }

        /// <summary>
        /// A method that runs the solver to get the result
        /// </summary>
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
                NewTripFound?.Invoke(this, new TripAddedEventArgs(newTrip));

                for (int s = 0; s < newTrip.Stops.Count; s++)
                    FrequencyMatrix[newTrip.Stops[s].Index - 1][s]++;

                AggregateTrips();
            }
        }

        /// <summary>
        /// A method that parses a list of cities into properties used by the solver
        /// </summary>
        /// <param name="cities">The cities to be parsed</param>
        public void OrderData(List<City> cities)
        {
            Origin = cities[0];
            Destinations = cities.Skip(1).ToList();
            SetFrequencyMatrix();
            GSolver.OrderData(cities);
        }

        /// <summary>
        /// A method that creates the common path based off of the frequency matrix
        /// </summary>
        private void AggregateTrips()
        {
            City[] stops = new City[Destinations.Count + 2];

            // Set the paths start and end
            stops[0] = stops[stops.Length - 1] = Origin;

            // Loop for each unassigned path position
            for (int s = 1; s < stops.Length - 1; s++)
            {
                // Find the highest frequency for the spot
                int hFreq = 0;
                for (int c = 1; c < Destinations.Count + 1; c++)
                {
                    int freq = FrequencyMatrix[c][s];

                    if (freq > hFreq)
                        hFreq = freq;
                }

                // List the potential cities for the spot
                List<City> pCities = Destinations.Where(city => FrequencyMatrix[city.Index - 1][s] == hFreq).ToList();

                // Add a city into the spot
                foreach (City pCity in pCities)
                {
                    if (!stops.Contains(pCity))
                        stops[s] = pCity;
                }
            }

            // Create the new common trip
            BestTrip = new Trip(stops.ToList());
            NewBestFound?.Invoke(this, new BestFoundEventArgs(0, BestTrip));
        }

        /// <summary>
        /// A method to initialize the frequency matrix
        /// </summary>
        private void SetFrequencyMatrix()
        {
            FrequencyMatrix = new int[Destinations.Count + 1][];
            for (int i = 0; i < FrequencyMatrix.Length; i++)
                FrequencyMatrix[i] = new int[Destinations.Count + 2];
        }

        /// <summary>
        /// A debug method to print the frequency matrix to the console
        /// </summary>
        private void PrintMatrix()
        {
            foreach (int[] col in FrequencyMatrix)
            {
                Console.Write("|");
                foreach (int row in col)
                    Console.Write($" {row} |");
                Console.Write("\n");
            }
            Console.Write("\n");

        }
    }
}
