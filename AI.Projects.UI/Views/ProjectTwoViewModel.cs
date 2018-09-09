using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using AI.Projects.Project1.Models;
using Caliburn.Micro;
using Microsoft.Win32;

namespace AI.Projects.UI.Views
{
    public class ProjectTwoViewModel : Screen
    {
        public FileInfo CurrentFile { get; set; }
        public City Origin { get; set; }
        public List<City> Destinations { get; set; }
        /// <summary>
        /// A method called by the UI to open a file dialog and select a test file
        /// </summary>
        public void BrowseFiles()
        {
            // Set the initial propertys for the file dialog
            var fileDialog = new OpenFileDialog
            {
                InitialDirectory = Path.GetFullPath("..\\"),
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
            GetCities();
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

    }
}
