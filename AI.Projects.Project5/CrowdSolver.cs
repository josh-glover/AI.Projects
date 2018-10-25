using System.Collections.Generic;
using AI.Projects.Shared.Interfaces;
using AI.Projects.Shared.Models;

namespace AI.Projects.Project5
{
    public class CrowdSolver : ISolver
    {
        public City Origin { get; set; }
        public List<City> Destinations { get; set; }

        public void GetShortestPath()
        {
            throw new System.NotImplementedException();
        }

        public void OrderData(List<City> cities)
        {
            throw new System.NotImplementedException();
        }
    }
}
