using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AI.Projects.Shared.Models;
using Caliburn.Micro;
using Microsoft.Win32;

namespace AI.Projects.UI.Views
{
    public class ProjectOneViewModel : Screen
    {
        /// <summary>
        /// A property that stores the starting point of the cycle
        /// </summary>
        public City Origin { get; set; }

        /// <summary>
        /// A property that stores a list of the points that need to be visited
        /// </summary>
        public List<City> Destinations { get; set; }

        /// <summary>
        /// A property that stores all permutations of a path between the Origin and Destinations points
        /// </summary>
        public List<Trip> Paths { get; set; }

        /// <summary>
        /// A property that stores the file being used for information
        /// </summary>
        public FileInfo CurrentFile { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ProjectOneViewModel()
        {
            // Initialize the lists
            Destinations = new List<City>();
            Paths = new List<Trip>();
        }

        /// <summary>
        /// A method called by the UI to open a file dialog and select a test file
        /// </summary>
        public void BrowseFiles()
        {
            // Set the initial propertys for the file dialog
            var fileDialog = new OpenFileDialog
            {
                InitialDirectory = Path.GetFullPath("..\\..\\..\\AI.Projects.Project1\\TestFiles"),
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "Test Files | *.tsp"
            };

            // Trap the user in a loop to ensure they pick a file
            do
            {
                // Display the dialog on the screen
                fileDialog.ShowDialog();

                // Save the selected file information as the current file
                CurrentFile = new FileInfo(fileDialog.FileName);
            } while (CurrentFile == null);

            // Notify the UI that the file has changed
            NotifyOfPropertyChange(nameof(CurrentFile));

            // Read the file to get city information
            GetCities();
        }

        /// <summary>
        /// A method called by the UI to calculate the shortest path
        /// </summary>
        public void GetShortestPath()
        {
            // Run the permutaion generation on a seperate thread to prevent program failure.
            var getPerm = Task.Run(() => GeneratePermutations(Destinations.ToList(), new List<City>()));

            // TODO: This functionality might work better using an event subscription to trigger the
            // TODO: shortest path method
            // Wait for the permutations task to close
            Task.WaitAll(getPerm);

            // Use the new permutations to find the shortest path
            FindShortestPath();
        }

        /// TODO: Could be made generic?
        /// <summary>
        /// A method that generates all permutations of a given list
        /// </summary>
        /// <param name="cities">The list that will be evaluated for permutations</param>
        /// <param name="result">A list used to store the current permutation</param>
        public void GeneratePermutations(List<City> cities, List<City> result)
        {
            // Ends the recursion loop if the result is the desired length
            if (result.Count == Destinations.Count)
            {
                // Adds this permutation to the list
                Paths.Add(new Trip(Origin, result));
                return;
            }

            // Loops through every city for this space in the permutation
            foreach (City city in cities)
            {
                // Adds the city to the current permutaion
                result.Add(city);

                // Removes the city from cities to prevent duplicates
                var tempCities = new List<City>(cities);
                tempCities.Remove(city);

                // Recursively calls itself to advance the position of the permutation
                GeneratePermutations(tempCities, result);

                // Remove the current city from the result to advance to the next point's variations
                result.Remove(city);
            }
        }

        /// <summary>
        /// A method that reads the cities from the current file
        /// </summary>
        private void GetCities()
        {
            // Makes sure the file is valid
            if (CurrentFile != null && !CurrentFile.Exists)
            {
                MessageBox.Show("There is no valid file selected", "No Valid File",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Clear any previous data
            Origin = null;
            Destinations.Clear();

            string[] temp;
            StreamReader reader = new StreamReader(CurrentFile.FullName);

            // Skip the first four lines
            for (int x = 0; x < 4; x++)
                reader.ReadLine();

            // Get the number of destinations
            temp = reader.ReadLine().Split(' ');
            int cityCount = int.Parse(temp[1]);

            // Skip two more lines
            for (int y = 0; y < 2; y++)
                reader.ReadLine();

            // Read the data for the origin city
            temp = reader.ReadLine().Split(' ');
            Origin = new City(int.Parse(temp[0]),
                              double.Parse(temp[1]),
                              double.Parse(temp[2]));

            // Read the data for each city
            for (int z = 0; z < cityCount - 1; z++)
            {
                temp = reader.ReadLine().Split(' ');
                Destinations.Add(new City(int.Parse(temp[0]),
                                          double.Parse(temp[1]),
                                          double.Parse(temp[2])));
            }
        }

        /// <summary>
        /// A method that finds the shortest path from the permutations
        /// </summary>
        /// <returns>The trip object that represents the shortest path</returns>
        private void FindShortestPath()
        {
            Trip shortest = null;

            // Loop through each cycle permutation and replace the shortest path if the
            // current cycle has a shorter distance
            foreach (Trip trip in Paths)
            {
                if (shortest == null) shortest = trip;

                if (trip.GetDistance() < shortest.GetDistance())
                    shortest = trip;
            }

            // Print the shortest paths route and total distance in a message box to the user
            var temp = "";
            foreach (City city in shortest.Stops)
                temp += $"{city.Index} ";

            MessageBox.Show($"{temp} - Distance: {shortest.GetDistance()}");
        }
    }
}
