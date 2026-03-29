using FAJARILLO_FINAL_PROJECT.CLASSES;
using System;
using System.Collections.Generic;
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
    public partial class FlightDetailsInput : Window
    {
        private string currentUser;
        public Flight Flight { get; private set; }
        public List<Flight> Flights { get; private set; } = new List<Flight>();

        public FlightDetailsInput()
        {
            InitializeComponent();
            FlightDatePicker.SelectedDate = DateTime.Now;
        }

        public FlightDetailsInput(string username)
        {
            InitializeComponent();
            currentUser = username;
            FlightDatePicker.SelectedDate = DateTime.Now;
        }

        public FlightDetailsInput(string username, Flight flight)
        {
            InitializeComponent();
            currentUser = username;

            Flight = flight;
            AircraftTypeBox.Text = flight.AircraftType;
            RegistrationBox.Text = flight.Registration;
            FlightDatePicker.SelectedDate = flight.FlightDate;
            HoursBox.Text = flight.Hours.ToString();
            MinutesBox.Text = flight.Minutes.ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string aircraft = AircraftTypeBox.Text.Trim();
            string registration = RegistrationBox.Text.Trim();
            DateTime? date = FlightDatePicker.SelectedDate;

            if (aircraft == "" || registration == "" || !date.HasValue)
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            int hours;
            int minutes;

            if (!IsAllDigits(HoursBox.Text.Trim()))
            {
                MessageBox.Show("Invalid hours value.");
                return;
            }
            hours = Convert.ToInt32(HoursBox.Text.Trim());
            if (hours < 0)
            {
                MessageBox.Show("Hours cannot be negative.");
                return;
            }

            if (!IsAllDigits(MinutesBox.Text.Trim()))
            {
                MessageBox.Show("Invalid minutes value.");
                return;
            }
            minutes = Convert.ToInt32(MinutesBox.Text.Trim());
            if (minutes < 0 || minutes > 59)
            {
                MessageBox.Show("Minutes must be between 0 and 59.");
                return;
            }

            if (Flight != null) // editing
            {
                Flight.AircraftType = aircraft;
                Flight.Registration = registration;
                Flight.FlightDate = date.Value;
                Flight.Hours = hours;
                Flight.Minutes = minutes;

                this.DialogResult = true;
                this.Close();
            }
            else // new flight
            {
                var newFlight = new Flight(aircraft, registration, date.Value, hours, minutes);
                Flights.Add(newFlight);

                var result = MessageBox.Show(
                    "Flight saved successfully!\n\nDo you want to add another entry?",
                    "Add Another?",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    AircraftTypeBox.Text = "";
                    RegistrationBox.Text = "";
                    FlightDatePicker.SelectedDate = DateTime.Now;
                    HoursBox.Text = "";
                    MinutesBox.Text = "";
                }
                else
                {
                    this.DialogResult = true;
                    this.Close();
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (Flights.Count > 0)
                this.DialogResult = true;
            else
                this.DialogResult = false;

            this.Close();
        }

        private bool IsAllDigits(string text)
        {
            if (text == "") return false;

            foreach (char c in text)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Save_Click(SaveButton, new RoutedEventArgs(Button.ClickEvent, SaveButton));
                e.Handled = true;
            }
        }
    }
}