using System;
using System.Collections.Generic;
using AI.Projects.Shared.Interfaces;
using AI.Projects.Shared.Models;

namespace AI.Projects.Project3
{
    public class ClosestEdgeSolver : ISolver
    {
        public City Origin { get; set; }
        public City FirstCity { get; set; }
        public List<City> Destinations { get; set; }

        public void OrderData(List<City> cities)
        {
            
        }

        public void GetShortestPath()
        {
            throw new NotImplementedException();
        }
    }
}
