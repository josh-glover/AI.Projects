using AI.Projects.Shared.Utilities;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace AI.Projects.Shared.Models
{
    public class Trip
    {
        /// <summary>
        /// A property that stores all the stops in this permutation of the route
        /// </summary>
        public List<City> Stops { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="origin">The origin city</param>
        /// <param name="dest">The cities that are visited in the route</param>
        public Trip(City origin, List<City> dest, bool cycle)
        {
            // Initialize the list and origin to the start
            Stops = new List<City> { origin };


            // Add each destination for this permutation
            foreach (City city in dest)
                Stops.Add(city);

            // Add the origin to the end to complete the cycle
            if(cycle)
                Stops.Add(origin);
        }

        /// <summary>
        /// A method that calculates the total distance of this path
        /// </summary>
        /// <returns>The total distance of this path</returns>
        public double GetDistance()
        {
            double distance = 0;

            // For every node except for the last, add the distance between the current
            // and next node to the total distance 
            for (int i = 0; i < Stops.Count - 1; i++)
                distance += Stops[i].DistanceTo(Stops[i + 1]);

            // Return the total distance
            return distance;
        }

        public override string ToString()
        {
            string result = "";
            foreach (City stop in Stops)
            {
                result += $"{stop.Index} ";
            }

            result += $"{GetDistance()}";
            return result;
        }
    }
}
