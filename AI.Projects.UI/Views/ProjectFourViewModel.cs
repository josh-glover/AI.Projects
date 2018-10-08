using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using AI.Projects.Project4;
using AI.Projects.Shared.Events;
using AI.Projects.Shared.Models;
using Caliburn.Micro;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Series;

namespace AI.Projects.UI.Views
{
    public class ProjectFourViewModel : Screen
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
        /// A property that stores the current best distance
        /// </summary>
        public int BestDistance { get; set; }
        /// <summary>
        /// A property that stores the model to control the graph UI
        /// </summary>
        public PlotModel PlotInfo { get; set; }
        /// <summary>
        /// A property that stores the model to control the graph UI
        /// </summary>
        public PlotModel ChangeInfo { get; set; }
        /// <summary>
        /// A property that stores information of the scatter plot
        /// </summary>
        public ScatterSeries CitySeries { get; set; }
        /// <summary>
        /// A property that stores the information of the graph connections
        /// </summary>
        public LineSeries PathSeries { get; set; }
        /// <summary>
        /// A property that stores the information of the graph changes
        /// </summary>
        public LineSeries ChangeSeries { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ProjectFourViewModel()
        {
            Cities = new List<City>();
            PlotInfo = new PlotModel();
            ChangeInfo = new PlotModel();
            CitySeries = new ScatterSeries();
            PathSeries = new LineSeries();
            ChangeSeries = new LineSeries();
            PlotInfo.Series.Add(PathSeries);
            PlotInfo.Series.Add(CitySeries);
            ChangeInfo.Series.Add(ChangeSeries);

            GSolver = new GeneticSolver();
            GSolver.NewBestTrip += OnNewBestTrip;
            GSolver.DataCleared += OnDataCleared;
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

            PlotInfo.InvalidatePlot(true);
            OnDataCleared(this, new EventArgs());
            GSolver.OrderData(Cities);
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
        /// A method called by the UI to start generating the paths
        /// </summary>
        public void StartGeneration()
        {
            Task.Run(() => { GSolver.GetShortestPath(); });
        }

        /// <summary>
        /// A method triggered by the NewBestTrip event that updates the new best path
        /// </summary>
        /// <param name="sender">The solver that sent the event</param>
        /// <param name="args">The event arguments</param>
        private void OnNewBestTrip(object sender, BestFoundEventArgs args)
        {
            PathSeries.Points.Clear();

            foreach (City point in args.NewBest.Stops)
                PathSeries.Points.Add(new DataPoint(point.XPosition, point.YPosition));

            ChangeSeries.Points.Add(new DataPoint(args.Generation, 1 / (args.NewBest.Fitness) + 1));

            BestDistance = (int)args.NewBest.GetDistance();

            NotifyOfPropertyChange(nameof(BestDistance));
            PlotInfo.InvalidatePlot(true);
            ChangeInfo.InvalidatePlot(true);
        }

        /// <summary>
        /// A method triggered by the DataCleared event that clears all point information
        /// </summary>
        /// <param name="sender">The solver that sent the event</param>
        /// <param name="args">The event arguments</param>
        private void OnDataCleared(object sender, EventArgs args)
        {
            PathSeries.Points.Clear();

            PlotInfo.InvalidatePlot(true);
        }
    }
}
