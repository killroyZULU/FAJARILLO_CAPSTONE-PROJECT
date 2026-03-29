using FAJARILLO_FINAL_PROJECT.CLASSES;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FAJARILLO_FINAL_PROJECT.WINDOWS
{
    public partial class LoggedFlights : Window
    {
        private string flightFile;
        private string currentUser;
        private Users loggedUser;
        private List<Flight> flights = new List<Flight>();

        public LoggedFlights(string username, Users user)
        {
            InitializeComponent();

            currentUser = username;
            loggedUser = user;
            flightFile = $"C:\\Users\\Lee Adrian\\source\\repos\\FAJARILLO_FINAL PROJECT\\FAJARILLO_FINAL PROJECT\\USER DATA\\{currentUser}_flights.txt";

            flights = FlightFileReadWrite.ReadFlights(username);
            LoadFlights();
            RefreshListView();
        }

        private void LoadFlights()
        {
            flights.Clear();

            if (File.Exists(flightFile))
            {
                using (StreamReader reader = new StreamReader(flightFile))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length == 5)
                        {
                            if (int.TryParse(parts[3], out int hours) &&
                                int.TryParse(parts[4], out int minutes) &&
                                DateTime.TryParse(parts[2], out DateTime flightDate))
                            {
                                flights.Add(new Flight
                                {
                                    AircraftType = parts[0],
                                    Registration = parts[1],
                                    FlightDate = flightDate,
                                    Hours = hours,
                                    Minutes = minutes
                                });
                            }
                        }
                    }
                }
            }

            RefreshListView();
        }

        private void SaveToFile()
        {
            using (StreamWriter sw = new StreamWriter(flightFile, false))
            {
                foreach (var f in flights)
                {
                    sw.WriteLine($"{f.AircraftType}|{f.Registration}|{f.FlightDate}|{f.Hours}|{f.Minutes}");
                }
            }
        }

        public void RefreshListView()
        {
            var items = new List<object>();
            foreach (var f in flights)
            {
                items.Add(new
                {
                    AircraftType = f.AircraftType,
                    Registration = f.Registration,
                    FlightDate = f.FlightDate.ToShortDateString(),
                    FlightTime = f.Hours + "h " + f.Minutes + "m"
                });
            }

            FlightListView.ItemsSource = items;
            UpdateTotalTime();
        }

        private void UpdateTotalTime()
        {
            int totalHours = 0;
            int totalMinutes = 0;

            foreach (var f in flights)
            {
                totalHours += f.Hours;
                totalMinutes += f.Minutes;
            }

            totalHours += totalMinutes / 60;
            totalMinutes %= 60;

            TotalTimeText.Text = "Total Flight Time: " + totalHours + "h " + totalMinutes + "m";
        }

        private void AddFlight_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FlightDetailsInput(currentUser);

            if (dialog.ShowDialog() == true)
            {
                foreach (var f in dialog.Flights)
                {
                    flights.Add(f);
                }
                SaveToFile();
                RefreshListView();
            }
        }

        private void EditFlight_Click(object sender, RoutedEventArgs e)
        {
            if (FlightListView.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a flight to edit.");
                return;
            }

            int index = FlightListView.SelectedIndex;
            var selected = flights[index];

            var dialog = new FlightDetailsInput(currentUser, selected);

            if (dialog.ShowDialog() == true && dialog.Flight != null)
            {
                flights[index] = dialog.Flight;
                SaveToFile();
                RefreshListView();
            }
        }

        private void DeleteFlight_Click(object sender, RoutedEventArgs e)
        {
            if (FlightListView.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a flight to delete.");
                return;
            }

            var result = MessageBox.Show("Are you sure you want to delete this flight?",
                                         "Confirm Deletion",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                flights.RemoveAt(FlightListView.SelectedIndex);
                SaveToFile();
                RefreshListView();
            }
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string choice = selectedItem.Content.ToString();

                for (int i = 0; i < flights.Count - 1; i++)
                {
                    for (int j = i + 1; j < flights.Count; j++)
                    {
                        bool swap = false;

                        if (choice == "Earliest First" && flights[i].FlightDate > flights[j].FlightDate)
                            swap = true;
                        else if (choice == "Latest First" && flights[i].FlightDate < flights[j].FlightDate)
                            swap = true;

                        if (swap)
                        {
                            Flight temp = flights[i];
                            flights[i] = flights[j];
                            flights[j] = temp;
                        }
                    }
                }

                RefreshListView();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Dashboard dashboardWindow = new Dashboard(loggedUser);
            dashboardWindow.Show();
            this.Close();
        }
    }
}
