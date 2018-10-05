using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using AI.Projects.Project3;
using AI.Projects.Project4;
using AI.Projects.Shared.Events;
using AI.Projects.Shared.Models;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Series;

namespace AI.Projects.UI.Views
{
    public class ProjectFourViewModel
    {
        /// <summary>
        /// A property that stores the solver that will perform the genetic algorithm
        /// </summary>
        public GeneticSolver GSolver { get; set; }
        /// <summary>
        /// A property that stores the file being used for information
        /// </summary>
        public FileInfo CurrentFile { get; set; }
        /// <summary>
        /// A property that stores a list of the points to be sent to a solver
        /// </summary>
        public List<City> Cities { get; set; }
        /// <summary>
        /// A property that stores the order that cities are added
        /// </summary>
        public ObservableCollection<City> AddedOrder { get; set; }
        /// <summary>
        /// A property that stores the model to control the graph UI
        /// </summary>
        public PlotModel PlotInfo { get; set; }
        /// <summary>
        /// A property that stores information of the scatter plot
        /// </summary>
        public ScatterSeries CitySeries { get; set; }
        /// <summary>
        /// A property that stores the information of the graph connections
        /// </summary>
        public LineSeries PlotSeries { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ProjectFourViewModel()
        {
            Cities = new List<City>();
            AddedOrder = new ObservableCollection<City>();
            PlotInfo = new PlotModel();
            CitySeries = new ScatterSeries();
            PlotSeries = new LineSeries();
            PlotInfo.Series.Add(PlotSeries);
            PlotInfo.Series.Add(CitySeries);
            GSolver = new GeneticSolver();
            GSolver.CityAdded += OnCityAdded;
            GSolver.DataCleared += OnDataCleared;
        }

        /// <summary>
        /// A method called by the UI to open a file dialog and select a test file
        /// </summary>
        public void BrowseFiles()
        {
            // Set the initial propertys for the file dialog
            var fileDialog = new OpenFileDialog
            {
                InitialDirectory = Path.GetFullPath("..\\..\\..\\AI.Projects.Project4\\TestFiles"),
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
                try
                {
                    CurrentFile = new FileInfo(fileDialog.FileName);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"File Error: {e.Message}", "File Error",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } while (CurrentFile == null);

            // Notify the UI that the file has changed
            NotifyOfPropertyChange(nameof(CurrentFile));

            // Read the file to get city information
            ReadFile();
        }

        /// <summary>
        /// A method called by the UI to add a the closest city to an edge
        /// </summary>
        public void AddCity()
        {
            GSolver.AddClosestCity();
        }

        /// <summary>
        /// A method that reads the cities from the current file
        /// </summary>
        private void ReadFile()
        {
            CitySeries.Points.Clear();
            Cities.Clear();
            // Makes sure the file is valid
            if (CurrentFile != null && !CurrentFile.Exists)
            {
                MessageBox.Show("There is no valid file selected", "No Valid File",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            StreamReader reader = new StreamReader(CurrentFile.FullName);

            // Skip the first four lines
            for (int x = 0; x < 4; x++)
                reader.ReadLine();

            // Get the number of destinations
            var temp = reader.ReadLine().Split(' ');
            int cityCount = int.Parse(temp[1]);

            // Skip two more lines
            for (int y = 0; y < 2; y++)
                reader.ReadLine();

            // Read the data for each city
            for (int z = 0; z < cityCount; z++)
            {
                temp = reader.ReadLine().Split(' ');
                var city = new City(int.Parse(temp[0]),
                                    double.Parse(temp[1]),
                                    double.Parse(temp[2]));
                // Add the city to the list
                Cities.Add(city);

                // Add the city to the graph
                CitySeries.Points.Add(new ScatterPoint(city.XPosition, city.YPosition));
            }

            GSolver.OrderData(Cities);
        }

        /// <summary>
        /// A method triggered by the CityAdded event that adds the point to the graph
        /// </summary>
        /// <param name="sender">The solver sending the message</param>
        /// <param name="args">The information about the city added</param>
        private void OnCityAdded(object sender, CityAddedEventArgs args)
        {
            // Add to the AddedOrder list to keep track of order
            AddedOrder.Add(args.AddedCity);

            // Plot the new point on the graph
            var point = new DataPoint(args.AddedCity.XPosition, args.AddedCity.YPosition);
            PlotSeries.Points.Insert(args.Index, point);

            // Update the UI
            NotifyOfPropertyChange(nameof(AddedOrder));
            PlotInfo.InvalidatePlot(true);
        }

        /// <summary>
        /// A method triggered by the DataCleared event that clears all point information
        /// </summary>
        /// <param name="sender">The solver that sent the event</param>
        /// <param name="args">The event arguments</param>
        private void OnDataCleared(object sender, EventArgs args)
        {
            AddedOrder.Clear();
            PlotSeries.Points.Clear();

            PlotInfo.InvalidatePlot(true);
        }

    }
}
