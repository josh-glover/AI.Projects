using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using AI.Projects.Project5;
using AI.Projects.Shared.Events;
using AI.Projects.Shared.Models;
using Caliburn.Micro;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Series;

namespace AI.Projects.UI.Views
{
    public class ProjectFiveViewModel : Screen
    {
        // Fields
        private Trip _selectedTrip;

        /// <summary>
        /// A property that stores the crowd solver to get the shortest path
        /// </summary>
        public CrowdSolver CSolver { get; set; }
        /// <summary>
        /// A property that stores the current distance of the common path
        /// </summary>
        public double BestDistance { get; set; }
        /// <summary>
        /// A property that stores the file being used for information
        /// </summary>
        public FileInfo CurrentFile { get; set; }
        /// <summary>
        /// A property that stores a list of the points to be sent to a solver
        /// </summary>
        public List<City> Cities { get; set; }
        /// <summary>
        /// A property that stores the selected trip in the trip tab
        /// </summary>
        public Trip SelectedTrip
        {
            get
            {
                return _selectedTrip;
            }
            set
            {
                _selectedTrip = value;

                SelectedPathSeries.Points.Clear();
                foreach (City point in _selectedTrip.Stops)
                    SelectedPathSeries.Points.Add(new DataPoint(point.XPosition, point.YPosition));

                SelectedPathInfo.InvalidatePlot(true);
            }
        }
        /// <summary>
        /// A property that stores a list of the generated trips
        /// </summary>
        public ObservableCollection<Trip> TripCollection { get; set; }
        /// <summary>
        /// A property that stores the model to control the graph UI
        /// </summary>
        public PlotModel BestPathInfo { get; set; }
        /// <summary>
        /// A property that stores the model to control the trips graph
        /// </summary>
        public PlotModel SelectedPathInfo { get; set; }
        /// <summary>
        /// A property that stores the model to control the graph UI
        /// </summary>
        public PlotModel ChangeInfo { get; set; }
        /// <summary>
        /// A property that stores information of the scatter plot
        /// </summary>
        public ScatterSeries BestTripCitySeries { get; set; }
        /// <summary>
        /// A property that stores information of the scatter plot
        /// </summary>
        public ScatterSeries SelectedTripCitySeries { get; set; }
        /// <summary>
        /// A property that stores the information of the graph connections
        /// </summary>
        public LineSeries BestPathSeries { get; set; }
        /// <summary>
        /// A property that stores the information of the selected trips connections
        /// </summary>
        public LineSeries SelectedPathSeries { get; set; }
        /// <summary>
        /// A property that stores the information of the graph changes
        /// </summary>
        public LineSeries ChangeSeries { get; set; }
        
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ProjectFiveViewModel()
        {
            BestDistance = 0;
            Cities = new List<City>();
            TripCollection = new ObservableCollection<Trip>();

            BestPathInfo = new PlotModel();
            SelectedPathInfo = new PlotModel();
            ChangeInfo = new PlotModel();

            BestTripCitySeries = new ScatterSeries();
            SelectedTripCitySeries = new ScatterSeries();
            BestPathSeries = new LineSeries();
            SelectedPathSeries = new LineSeries();
            ChangeSeries = new LineSeries();

            BestPathInfo.Series.Add(BestPathSeries);
            BestPathInfo.Series.Add(BestTripCitySeries);
            SelectedPathInfo.Series.Add(SelectedPathSeries);
            SelectedPathInfo.Series.Add(SelectedTripCitySeries);
            ChangeInfo.Series.Add(ChangeSeries);

            CSolver = new CrowdSolver();
            CSolver.NewBestFound += OnNewBestFound;
            CSolver.NewTripFound += OnNewTripFound;
        }

        /// <summary>
        /// A method that reads the cities from the current file
        /// </summary>
        private void ReadFile()
        {
            BestTripCitySeries.Points.Clear();
            SelectedTripCitySeries.Points.Clear();
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
                BestTripCitySeries.Points.Add(new ScatterPoint(city.XPosition, city.YPosition));
                SelectedTripCitySeries.Points.Add(new ScatterPoint(city.XPosition, city.YPosition));
            }

            BestPathInfo.InvalidatePlot(true);
            SelectedPathInfo.InvalidatePlot(true);

            CSolver.OrderData(Cities);
        }

        /// <summary>
        /// A method called by the UI to open a file dialog and select a test file
        /// </summary>
        public void BrowseFiles()
        {
            // Set the initial propertys for the file dialog
            var fileDialog = new OpenFileDialog
            {
                InitialDirectory = Path.GetFullPath("..\\..\\..\\AI.Projects.Project5\\TestFiles"),
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
            TripCollection.Clear();
            Task.Run(() => { CSolver.GetShortestPath(); });
        }
        
        /// <summary>
        /// A method called when a new solution is generated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnNewTripFound(object sender, TripAddedEventArgs args)
        {
            OnUIThread(() => { TripCollection.Add(args.NewTrip); });
            NotifyOfPropertyChange();
        }

        /// <summary>
        /// A method called when a new common path is created
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnNewBestFound(object sender, BestFoundEventArgs args)
        {
            BestDistance = args.NewBest.GetDistance();
            BestPathSeries.Points.Clear();

            foreach (City stop in args.NewBest.Stops)
                BestPathSeries.Points.Add(new DataPoint(stop.XPosition, stop.YPosition));

            BestPathInfo.InvalidatePlot(true);
            NotifyOfPropertyChange(nameof(BestDistance));
        }
    }
}
