using System;
using AI.Projects.Project1.Models;

namespace AI.Projects.Project1.Utilities
{
    public static class CityExtensions
    {
        /// <summary>
        /// A extension method to calculate the distance between two cities
        /// </summary>
        /// <param name="o">The origin city</param>
        /// <param name="d">The destination city</param>
        /// <returns>The distance between the two points</returns>
        public static double DistanceTo(this City o, City d)
        {
            // Return the result of the distance formula
            return Math.Sqrt(Math.Pow(d.YPosition - o.YPosition, 2) + Math.Pow(d.XPosition - o.XPosition, 2));
        }
    }
}
