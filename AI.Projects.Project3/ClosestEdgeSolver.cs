using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
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
        public EventHandler<EventArgs> DataCleared;
        public City Origin { get; set; }
        public City SecondCity { get; set; }
        public List<City> Destinations { get; set; }
        public List<City> VisitedCities { get; set; }

        public ClosestEdgeSolver()
        {
            Destinations = new List<City>();
            VisitedCities = new List<City>();
        }

        public void OrderData(List<City> cities)
        {
            Destinations.Clear();
            VisitedCities.Clear();
            DataCleared.Invoke(this, new EventArgs());

            Origin = cities[0];
            VisitedCities.Add(Origin);
            CityAdded.Invoke(this, new CityAddedEventArgs(Origin, 0));
            VisitedCities.Add(Origin);
            CityAdded.Invoke(this, new CityAddedEventArgs(Origin, 1));

            SecondCity = cities[1];
            foreach (City city in cities.Skip(2))
                if (Origin.DistanceTo(city) < Origin.DistanceTo(SecondCity))
                    SecondCity = city;

            VisitedCities.Insert(1, SecondCity);
            CityAdded.Invoke(this, new CityAddedEventArgs(SecondCity, 1));

            Destinations = cities.Where(c => c != Origin && c != SecondCity).ToList();
        }

        public void GetShortestPath()
        {
            Trip trip = new Trip(VisitedCities);
            MessageBox.Show($"Path: {trip}\n");
        }

        public void AddClosestCity()
        {
            var options = Destinations.Where(c => !VisitedCities.Contains(c)).ToList();

            if (options.Count == 0)
            {
                GetShortestPath();
                return;
            };

            City lineStart, lineEnd;
            Tuple<City, City, City> closest = new Tuple<City, City, City>(options[0], VisitedCities[0], VisitedCities[1]);

            foreach (City city in options.Skip(1))
            {
                for (int i = 0; i < VisitedCities.Count - 1; i++)
                {
                    lineStart = VisitedCities[i];
                    lineEnd = VisitedCities[i + 1];
                    Console.WriteLine($"Point: {city.Index}\n" +
                        $"Line: {lineStart.Index} to {lineEnd.Index}\n" +
                        $"IsClosest: {DistanceToEdge(city, lineStart, lineEnd)}" +
                        $" <= {DistanceToEdge(closest.Item1, closest.Item2, closest.Item3)}");
                    if (DistanceToEdge(city, lineStart, lineEnd) <=
                        DistanceToEdge(closest.Item1, closest.Item2, closest.Item3))
                    {
                        closest = new Tuple<City, City, City>(city, lineStart, lineEnd);
                    }
                }
            }

            if (closest.Item3 == Origin)
            {
                VisitedCities.Insert(VisitedCities.Count - 1, closest.Item1);
                CityAdded.Invoke(this, new CityAddedEventArgs(closest.Item1, VisitedCities.Count - 2));
            }
            else
            {
                VisitedCities.Insert(VisitedCities.IndexOf(closest.Item3), closest.Item1);
                CityAdded.Invoke(this, new CityAddedEventArgs(closest.Item1, VisitedCities.IndexOf(closest.Item3)));
            }
            //string result = "[ ";
            //foreach (int index in VisitedCities.Select(c => c.Index))
            //    result += $"{index} ";
            //MessageBox.Show($"{result} ]");
        }

        private double DistanceToEdge(City point, City lineStart, City lineEnd)
        {
            double lineDistance = lineStart.DistanceTo(lineEnd);
            double pNumerator = (lineEnd.XPosition - lineStart.XPosition) * (lineStart.YPosition - point.YPosition)
                                - (lineStart.XPosition - point.XPosition) * (lineEnd.YPosition - lineStart.YPosition);

            return Math.Abs(pNumerator) / lineDistance;
        }
    }
}
