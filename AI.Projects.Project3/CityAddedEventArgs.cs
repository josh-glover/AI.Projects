using System;
using AI.Projects.Shared.Models;

namespace AI.Projects.Project3
{
    public class CityAddedEventArgs : EventArgs
    {
        public City AddedCity { get; set; }

        public CityAddedEventArgs(City addedCity)
        {
            AddedCity = addedCity;
        }
    }
}
