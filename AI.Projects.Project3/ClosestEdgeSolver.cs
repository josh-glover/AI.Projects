using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AI.Projects.Shared.Events;
using AI.Projects.Shared.Interfaces;
using AI.Projects.Shared.Models;
using AI.Projects.Shared.Utilities;

namespace AI.Projects.Project3
{
    public class ClosestEdgeSolver : ISolver
    {
        // Event Handlers
        public EventHandler<CityAddedEventArgs> CityAdded;
        public EventHandler<EventArgs> DataCleared;

        /// <summary>
        /// A property that stores the starting point of the path
        /// </summary>
        public City Origin { get; set; }
        /// <summary>
        /// A property that stores the second point of the path
        /// </summary>
        public City SecondCity { get; set; }
        /// <summary>
        /// A property that stores a list of the points that can be visited
        /// </summary>
        public List<City> Destinations { get; set; }
        /// <summary>
        /// A property that stores a list of the cities visited during the search
        /// </summary>
        public List<City> VisitedCities { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ClosestEdgeSolver()
        {
            Destinations = new List<City>();
            VisitedCities = new List<City>();
        }

        /// <summary>
        /// A method that parses a list of cities into properties used by the solver
        /// </summary>
        /// <param name="cities">The cities to be parsed</param>
        public void OrderData(List<City> cities)
        {
            // Clear any previous information
            Destinations.Clear();
            VisitedCities.Clear();
            DataCleared.Invoke(this, new EventArgs());

            // Set the first city as the start and end
            Origin = cities[0];
            VisitedCities.Add(Origin);
            CityAdded.Invoke(this, new CityAddedEventArgs(Origin, 0));
            VisitedCities.Add(Origin);
            CityAdded.Invoke(this, new CityAddedEventArgs(Origin, 1));

            // Set the next closest node as the second point
            SecondCity = cities[1];

            foreach (City city in cities.Skip(2))
                if (Origin.DistanceTo(city) < Origin.DistanceTo(SecondCity))
                    SecondCity = city;

            VisitedCities.Insert(1, SecondCity);
            CityAdded.Invoke(this, new CityAddedEventArgs(SecondCity, 1));

            // Add the rest of the cities to the destinations list
            Destinations = cities.Where(c => c != Origin && c != SecondCity).ToList();
        }

        /// <summary>
        /// A method that displays the final information of the path
        /// </summary>
        public void GetShortestPath()
        {
            Trip trip = new Trip(VisitedCities);
            MessageBox.Show($"Path: {trip}\n");
        }

        /// <summary>
        /// A method that finds a city closest to an edge
        /// </summary>
        public void AddClosestCity()
        {
            // Get all the unvisited nodes
            var options = Destinations.Where(c => !VisitedCities.Contains(c)).ToList();

            // If there are no nodes left, display results and return
            if (options.Count == 0)
            {
                GetShortestPath();
                return;
            };

            City lineStart, lineEnd;
            Tuple<City, City, City> closest = new Tuple<City, City, City>(options[0], VisitedCities[0], VisitedCities[1]);

            // Iterate for each unvisited node
            foreach (City city in options.Skip(1))
            {
                // Iterate for each edge
                for (int i = 0; i < VisitedCities.Count - 1; i++)
                {
                    // Get the start and end of the connection
                    lineStart = VisitedCities[i];
                    lineEnd = VisitedCities[i + 1];

                    // If the distance to this connection is shorter, make it the shortest
                    if (DistanceToEdge(city, lineStart, lineEnd) <
                        DistanceToEdge(closest.Item1, closest.Item2, closest.Item3))
                    {
                        closest = new Tuple<City, City, City>(city, lineStart, lineEnd);
                    }
                }
            }

            // Set the city to visited, and add it to the UI graph
            if (closest.Item3 == Origin)
            {
                VisitedCities.Insert(VisitedCities.Count - 1, closest.Item1);
                CityAdded.Invoke(this, new CityAddedEventArgs(closest.Item1, VisitedCities.Count - 2));
            }
            else
            {
                VisitedCities.Insert(VisitedCities.IndexOf(closest.Item3), closest.Item1);
                CityAdded.Invoke(this, new CityAddedEventArgs(closest.Item1, VisitedCities.IndexOf(closest.Item1)));
            }
        }

        /// <summary>
        /// A method that calculates the distance from a point to a line
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="lineStart">The point that line starts from</param>
        /// <param name="lineEnd">The point the line ends at</param>
        /// <returns></returns>
        private double DistanceToEdge(City point, City lineStart, City lineEnd)
        {
            // Calculates the X and Y differences in the line
            double deltaX = lineEnd.XPosition - lineStart.XPosition;
            double deltaY = lineEnd.YPosition - lineStart.YPosition;

            // Calculate the translation to the line between the point and the line
            double t = ((point.XPosition - lineStart.XPosition) * deltaX
                       + (point.YPosition - lineStart.YPosition) * deltaY)
                       / (deltaY * deltaY + deltaX * deltaX);

            if (t < 0)
                t = 0;
            if (t > 1)
                t = 1;

            // Calculate the points on the line that is closest to the point
            double tempX = lineStart.XPosition + t * deltaX;
            double tempY = lineStart.YPosition + t * deltaY;

            // Calculate the X and Y differences of the new line
            deltaX = point.XPosition - tempX;
            deltaY = point.YPosition - tempY;

            // Return the distance of the new line
            return Math.Sqrt(deltaY * deltaY + deltaX * deltaX);
        }
    }
}
