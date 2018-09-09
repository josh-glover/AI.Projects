using System.Collections.Generic;
using AI.Projects.Project1.Models;
using AI.Projects.Shared.Interfaces;

namespace AI.Projects.Project2
{
    public class BreadthFirstSolver : ISolver
    {
        public City Origin { get; set; }
        public List<City> Destinations { get; set; }
        public City Goal { get; set; }

        public void GetShortestPath()
        {
            throw new System.NotImplementedException();
        }
    }
}
