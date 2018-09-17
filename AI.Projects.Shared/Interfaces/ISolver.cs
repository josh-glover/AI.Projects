using AI.Projects.Shared.Models;
using System.Collections.Generic;

namespace AI.Projects.Shared.Interfaces
{
    public interface ISolver
    {
        /// <summary>
        /// A property that stores the starting point of the path
        /// </summary>
        City Origin { get; set; }
        /// <summary>
        /// A property that stores a list of the points that can be visited
        /// </summary>
        List<City> Destinations { get; set; }

        /// <summary>
        /// A method that parses a list of cities into properties used by the solver
        /// </summary>
        /// <param name="cities">The cities to be parsed</param>
        void OrderData(List<City> cities);
        /// <summary>
        /// A method that runs the solver to get the result
        /// </summary>
        void GetShortestPath();
    }
}
