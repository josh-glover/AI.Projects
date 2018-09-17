using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using AI.Projects.Shared.Interfaces;
using AI.Projects.Shared.Models;

namespace AI.Projects.Project2
{
    public class DepthFirstSolver : ISolver
    {
        /// <summary>
        /// A property used to store the clock that times the runtime of the solver
        /// </summary>
        public Stopwatch Clock { get; set; }
        /// <summary>
        /// A property that stores the starting point of the path
        /// </summary>
        public City Origin { get; set; }
        /// <summary>
        /// A property that stores a list of the points that can be visited
        /// </summary>
        public List<City> Destinations { get; set; }
        /// <summary>
        /// A property that stores the ending point of the path
        /// </summary>
        public City Goal { get; set; }
        /// <summary>
        /// A property that stores a stack that evaluates the routes to be taken in DFS
        /// </summary>
        public Stack<Vertex> CityStack { get; set; }
        /// <summary>
        /// A property that stores a list of the cities visited during the search
        /// </summary>
        public List<Vertex> VisitedCities { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public DepthFirstSolver()
        {
            Clock = new Stopwatch();
            CityStack = new Stack<Vertex>();
            VisitedCities = new List<Vertex>();
        }

        /// <summary>
        /// A method that parses a list of cities into properties used by the solver
        /// </summary>
        /// <param name="cities">The cities to be parsed</param>
        public void OrderData(List<City> cities)
        {
            // Validate that there are more than 3 cities
            if (cities.Count < 3) return;

            // Initialize the cities
            Origin = cities[0];
            Destinations = cities.Skip(1).Take(cities.Count - 2).ToList();
            Goal = cities[cities.Count - 1];

            // Initialize the routes between cities
            // Currently hardcoded
            Origin.Routes.Add(Destinations[0]);             // From 1 to 2
            Origin.Routes.Add(Destinations[1]);             // From 1 to 3
            Origin.Routes.Add(Destinations[2]);             // From 1 to 4

            Destinations[0].Routes.Add(Destinations[1]);    // From 2 to 3

            Destinations[1].Routes.Add(Destinations[2]);    // From 3 to 4
            Destinations[1].Routes.Add(Destinations[3]);    // From 3 to 5

            Destinations[2].Routes.Add(Destinations[3]);    // From 4 to 5
            Destinations[2].Routes.Add(Destinations[4]);    // From 4 to 6
            Destinations[2].Routes.Add(Destinations[5]);    // From 4 to 7

            Destinations[3].Routes.Add(Destinations[5]);    // From 5 to 7
            Destinations[3].Routes.Add(Destinations[6]);    // From 5 to 8

            Destinations[4].Routes.Add(Destinations[6]);    // From 6 to 8

            Destinations[5].Routes.Add(Destinations[7]);    // From 7 to 9
            Destinations[5].Routes.Add(Destinations[8]);    // From 7 to 10

            Destinations[6].Routes.Add(Destinations[7]);    // From 8 to 9
            Destinations[6].Routes.Add(Destinations[8]);    // From 8 to 10
            Destinations[6].Routes.Add(Goal);               // From 8 to 11

            Destinations[7].Routes.Add(Goal);               // From 9 to 11

            Destinations[8].Routes.Add(Goal);               // From 10 to 11
        }

        /// <summary>
        /// A method that runs the solver to get the result
        /// </summary>
        public void GetShortestPath()
        {
            // Initialize for new calculations
            VisitedCities.Clear();
            Vertex tempVertex;

            // Start the timer
            Clock.Start();

            // Set the initial city into the stack
            Vertex currentCity = new Vertex
            {
                Point = Origin,
                Parent = null
            };
            CityStack.Push(currentCity);
            VisitedCities.Add(currentCity);

            // Iterate as long as there is a city in the stack
            while (CityStack.Count != 0)
            {
                // Get the next city in the stack
                currentCity = CityStack.Pop();

                // If the goal is found, then end the search
                if (currentCity.Point == Goal) break;

                // Iterate for each route a city can take to another
                foreach (City route in currentCity.Point.Routes)
                {
                    // If the city is a duplicate, skip this iteration
                    if (VisitedCities.Select(v => v.Point).Contains(route)) continue;

                    // If its a new city add it to the stack
                    tempVertex = new Vertex
                    {
                        Point = route,
                        Parent = currentCity.Point
                    };
                    CityStack.Push(tempVertex);
                    VisitedCities.Add(tempVertex);
                }
            }

            // Trace back the shortest path
            List<City> cities = new List<City>();
            while (currentCity.Parent != null)
            {
                cities.Add(currentCity.Point);
                currentCity = VisitedCities.FirstOrDefault(c => c.Point == currentCity.Parent);
            }
            var shortestPath = new Trip(Origin, cities.OrderBy(c => c.Index).ToList(), false);

            // Stop the timer and display the result
            Clock.Stop();
            MessageBox.Show($"Time: {Clock.Elapsed}\n" +
                            $"Path: {shortestPath}");
        }
    }
}
