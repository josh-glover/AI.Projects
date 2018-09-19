using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using AI.Projects.Shared.Interfaces;
using AI.Projects.Shared.Models;
using AI.Projects.Shared.Utilities;
using OxyPlot;
using OxyPlot.Series;

namespace AI.Projects.Project3
{
    public class ClosestEdgeSolver : ISolver
    {
        public EventHandler<CityAddedEventArgs> CityAdded;

        public City Origin { get; set; }
        public City SecondCity { get; set; }
        public List<City> Destinations { get; set; }
        public List<City> VisitedCities { get; set; }

        public void OrderData(List<City> cities)
        {
            Destinations = new List<City>();
            VisitedCities = new List<City>();

            Origin = cities[0];
            VisitedCities.Add(Origin);
            CityAdded.Invoke(this, new CityAddedEventArgs(Origin));

            SecondCity = cities[1];
            foreach (City city in cities.Skip(2))
                if (Origin.DistanceTo(city) < Origin.DistanceTo(SecondCity))
                    SecondCity = city;

            VisitedCities.Add(SecondCity);
            CityAdded.Invoke(this, new CityAddedEventArgs(SecondCity));

            Destinations = cities.Where(c => c != Origin && c != SecondCity).ToList();
        }

        public void GetShortestPath()
        {

        }

        public void AddClosestCity()
        {
            var options = Destinations.Where(c => !VisitedCities.Contains(c)).ToList();
            City lineStart, lineEnd;
            Tuple<City, City, City> closest = new Tuple<City, City, City>(options[0], VisitedCities[0], VisitedCities[1]);

            foreach (City city in options.Skip(1))
            {
                for (int i = 0; i < VisitedCities.Count - 1; i += 2)
                {
                    lineStart = VisitedCities[i];
                    lineEnd = VisitedCities[i + 1];

                    if (DistanceToEdge(city, lineStart, lineEnd) <
                        DistanceToEdge(closest.Item1, closest.Item2, closest.Item3))
                    {
                        closest = new Tuple<City, City, City>(city, lineStart, lineEnd);
                    }
                }
            }


        }

        private double DistanceToEdge(City point, City lineStart, City lineEnd)
        {
            double numerator = (lineEnd.XPosition - lineStart.XPosition) * (lineStart.YPosition - point.YPosition)
                               + (lineEnd.YPosition - lineStart.YPosition) * (lineStart.XPosition - point.XPosition);

            double denominator = Math.Pow((lineEnd.XPosition - lineStart.XPosition), 2)
                                 + Math.Pow((lineEnd.YPosition - lineStart.YPosition), 2);

            return Math.Abs(numerator) / Math.Sqrt(denominator);
        }
    }
}
