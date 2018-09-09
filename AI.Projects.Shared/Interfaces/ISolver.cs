using AI.Projects.Shared.Models;
using System.Collections.Generic;

namespace AI.Projects.Shared.Interfaces
{
    public interface ISolver
    {
        City Origin { get; set; }
        List<City> Destinations { get; set; }

        void OrderData(List<City> cities);
        void GetShortestPath();
    }
}
