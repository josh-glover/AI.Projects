using System;
using System.Collections.Generic;
using AI.Projects.Shared.Events;
using AI.Projects.Shared.Models;

namespace AI.Projects.Project4
{
    public class GeneticSolver
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

    }
}
