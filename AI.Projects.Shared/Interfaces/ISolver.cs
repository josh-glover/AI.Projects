using AI.Projects.Project1.Models;
using System.Collections.Generic;

namespace AI.Projects.Shared.Interfaces
{
    public interface ISolver
    {
        City Origin { get; set; }
        List<City> Destinations { get; set; }
        void GetShortestPath();
    }
}
