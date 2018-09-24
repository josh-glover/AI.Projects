using System;
using AI.Projects.Shared.Models;

namespace AI.Projects.Project3
{
    public class CityAddedEventArgs : EventArgs
    {
        /// <summary>
        /// A property that stores the added city
        /// </summary>
        public City AddedCity { get; set; }
        /// <summary>
        /// A property that stores the index the city was added
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="addedCity">The city being added</param>
        /// <param name="index">The index the city is being added to</param>
        public CityAddedEventArgs(City addedCity, int index)
        {
            AddedCity = addedCity;
            Index = index;
        }
    }
}
