using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using AI.Projects.Project4.Annotations;
using AI.Projects.Shared.Events;
using AI.Projects.Shared.Interfaces;
using AI.Projects.Shared.Models;

namespace AI.Projects.Project4
{
    public class GeneticSolver : ISolver, INotifyPropertyChanged
    {
        // Fields
        private bool _running;
        private int _generation;
        private Trip _best;
        public event PropertyChangedEventHandler PropertyChanged;

        public EventHandler<BestFoundEventArgs> NewBestTrip { get; set; }
        public EventHandler<EventArgs> DataCleared;

        /// <summary>
        /// A property that indicates the status of the algorithm
        /// </summary>
        public bool Running
        {
            get { return _running; }
            set
            {
                _running = value;
                NotifyOfPropertyChanged(nameof(Running));
            }
        }
        /// <summary>
        /// A property that stores the starting point of the path
        /// </summary>
        public City Origin { get; set; }
        /// <summary>
        /// A property that stores a list of the points that can be visited
        /// </summary>
        public List<City> Destinations { get; set; }
        /// <summary>
        /// A property that stores the current best solution
        /// </summary>
        public Trip BestTrip
        {
            get { return _best; }
            set
            {
                _best = value;
                NotifyOfPropertyChanged(nameof(BestTrip));
            }
        }
        /// <summary>
        /// A property that tracks the current generation
        /// </summary>
        public int Generation
        {
            get { return _generation; }
            set
            {
                _generation = value;
                NotifyOfPropertyChanged(nameof(Generation));
            }
        }
        /// <summary>
        /// A property that stores the current size of the population
        /// </summary>
        public int PopulationSize { get; set; }
        /// <summary>
        /// A property that stores the current population
        /// </summary>
        public List<Trip> Population { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public GeneticSolver()
        {
            Running = false;
            Destinations = new List<City>();
            Population = new List<Trip>();
        }

        /// <summary>
        /// A method that parses a list of cities into properties used by the solver
        /// </summary>
        /// <param name="cities">The cities to be parsed</param>
        public void OrderData(List<City> cities)
        {
            // Clear any previous information
            PopulationSize = 10;
            Destinations.Clear();

            // Set the first city as the start and end
            Origin = cities[0];

            // Add the rest of the cities to the destinations list
            Destinations = cities.Where(c => c != Origin).ToList();
        }

        /// <summary>
        /// A method that runs the solver to get the result
        /// </summary>
        public void GetShortestPath()
        {
            Random random = new Random();
            Running = true;
            ResetSolver();

            int bestCount = 0;
            GenerateInitialPopulation();

            while (Running)
            {
                //if (Generation == 499999)
                //    Running = false;

                GenerateNewPopulation();

                if (SortPopulation())
                    bestCount = 0;
                bestCount++;

                if (PopulationSize >= 320)
                    Genocide();
            }
        }

        /// <summary>
        /// A method that randomly generates the first 10 trips
        /// </summary>
        private void GenerateInitialPopulation()
        {
            Random random = new Random();

            for (int x = 0; x < PopulationSize; x++)
            {
                List<City> route = Destinations;

                for (int i = 0; i < route.Count; i++)
                {
                    int nIndex = random.Next(route.Count);
                    City temp = route[i];
                    route[i] = route[nIndex];
                    route[nIndex] = temp;
                }

                var path = new Trip(Origin, route, true);
                Population.Add(path);
            }

            Generation++;
            SortPopulation();
        }

        /// <summary>
        /// A method that generates new paths based on the current population
        /// </summary>
        private void GenerateNewPopulation()
        {
            Random random = new Random();
            int added = PopulationSize;
            int oldSize = PopulationSize;
            PopulationSize *= 2;
            Generation++;

            while (added > 0)
            {
                for (int x = 0; x < oldSize; x++)
                {
                    for (int y = x + 1; y < oldSize; y++)
                    {
                        double mutationSelect = random.NextDouble();
                        double crossSelect = random.NextDouble();
                        double crossChance = 0.075 * (10 * ((oldSize - x) / oldSize));

                        if (crossSelect <= crossChance / 2)
                        {
                            Trip newTrip = MixPop(Population[x], Population[y]);
                            if (mutationSelect <= 0.2)
                                newTrip = NeighborSwap(newTrip);
                            else if (mutationSelect > 0.2 && mutationSelect < 0.4)
                                newTrip = RandomizePoint(newTrip);

                            Population.Add(newTrip);
                            added--;
                        }
                        else if (crossSelect > crossChance / 2 && crossSelect <= crossChance)
                        {
                            Trip newTrip = SlicePop(Population[x], Population[y]);
                            if (mutationSelect <= 0.25)
                                newTrip = NeighborSwap(newTrip);
                            else if (mutationSelect > 0.2 && mutationSelect < 0.5)
                                newTrip = RandomizePoint(newTrip);

                            Population.Add(newTrip);
                            added--;
                        }

                        if (added == 0)
                            return;
                    }
                }
            }
        }

        #region Crossover Functions

        /// <summary>
        /// A method that randomly chooses points from the most fit path to be in the new path
        /// and fills the rest with point of the second
        /// </summary>
        /// <param name="parentOne">The first path</param>
        /// <param name="parentTwo">The second path</param>
        /// <returns>Returns the new path</returns>
        private Trip MixPop(Trip parentOne, Trip parentTwo)
        {
            Random random = new Random();
            List<City> newPath = new List<City>();

            if (parentOne.Fitness > parentTwo.Fitness)
            {
                foreach (City fitCity in parentOne.Stops)
                {
                    if (random.Next(100) + 1 < 60)
                        newPath.Add(fitCity);
                }

                foreach (City city in parentTwo.Stops)
                {
                    if (!newPath.Contains(city))
                        newPath.Add(city);
                }
            }
            else
            {
                foreach (City fitCity in parentTwo.Stops)
                {
                    if (random.Next(100) + 1 < 60)
                        newPath.Add(fitCity);
                }

                foreach (City city in parentOne.Stops)
                {
                    if (!newPath.Contains(city))
                        newPath.Add(city);
                }
            }

            newPath.Add(newPath[0]);

            return new Trip(newPath);
        }

        /// <summary>
        /// A method that randomly splits the first path and fills the rest with points from
        /// the second path
        /// </summary>
        /// <param name="parentOne">The first path</param>
        /// <param name="parentTwo">The second path</param>
        /// <returns>Returns the new path</returns>
        private Trip SlicePop(Trip parentOne, Trip parentTwo)
        {
            Random random = new Random();
            List<City> newPath = new List<City>();
            int index = random.Next(1, parentOne.Stops.Count - 1);

            newPath.AddRange(parentOne.Stops.Take(index));

            foreach (City city in parentTwo.Stops)
            {
                if (!newPath.Contains(city))
                    newPath.Add(city);
            }

            newPath.Add(newPath[0]);

            return new Trip(newPath);
        }

        #endregion

        #region Mutation Functions

        /// <summary>
        /// A mutation function that swaps a random point with the right neighbor.
        /// If the neighbor is out of bounds it cycles to the begining of the list.
        /// </summary>
        /// <param name="trip">The path being mutated</param>
        /// <returns>The mutated path</returns>
        private Trip NeighborSwap(Trip trip)
        {
            Random random = new Random();
            Trip newTrip = new Trip(new List<City>(trip.Stops));
            int index = random.Next(1, newTrip.Stops.Count - 1);
            City stop = newTrip.Stops[index];

            // Calculate the neighbors index
            if (index + 1 == newTrip.Stops.Count)
                index = 1;
            else
                index++;

            // Remove the random point
            newTrip.Stops.Remove(stop);
            // Insert the random point in the place of the neighbor
            newTrip.Stops.Insert(index, stop);

            return newTrip;
        }

        /// <summary>
        /// A mutation function that inserts a random point from the path  into a random index
        /// </summary>
        /// <param name="trip">The path being mutated</param>
        /// <returns>The mutated path</returns>
        private Trip RandomizePoint(Trip trip)
        {
            Random random = new Random();
            Trip newTrip = new Trip(new List<City>(trip.Stops));
            int index = random.Next(1, newTrip.Stops.Count - 1);
            City stop = newTrip.Stops[index];
            index = random.Next(1, newTrip.Stops.Count - 1);

            // Remove the random point
            newTrip.Stops.Remove(stop);
            // Insert the point into the random index
            newTrip.Stops.Insert(index, stop);

            return newTrip;
        }

        #endregion

        /// <summary>
        /// A method that kills off all but 10 paths
        /// </summary>
        private void Genocide()
        {
            Population = Population.OrderBy(t => t.Fitness).ToList();
            Population = Population.Take(10).ToList();
            PopulationSize = 10;
        }

        /// <summary>
        /// A method that sorts the population by its fitness
        /// </summary>
        /// <returns>Returns true if the best path changes</returns>
        private bool SortPopulation()
        {
            Trip oldBest = Population[0];

            Population = Population.OrderBy(t => t.Fitness).ToList();

            if (Population[0] == oldBest) return false;

            BestTrip = Population[0];
            NewBestTrip.Invoke(this, new BestFoundEventArgs(Generation, BestTrip));
            return true;
        }

        /// <summary>
        /// A method that resets the solver to be used
        /// </summary>
        private void ResetSolver()
        {
            BestTrip = new Trip(new List<City>());
            Population = new List<Trip>();
            PopulationSize = 10;
            Generation = 0;
            NotifyOfPropertyChanged(nameof(Generation));
            DataCleared.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// A method that notifies the UI of a property update
        /// </summary>
        /// <param name="propertyName">The property being updated</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyOfPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
