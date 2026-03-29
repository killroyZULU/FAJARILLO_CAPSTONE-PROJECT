using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAJARILLO_FINAL_PROJECT.CLASSES
{
    public class Flight
    {
        public string AircraftType { get; set; }
        public string Registration { get; set; }
        public DateTime FlightDate { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public Flight(string aircraftType, string registration, DateTime flightDate, int hours, int minutes)
        {
            AircraftType = aircraftType;
            Registration = registration;
            FlightDate = flightDate;
            Hours = hours;
            Minutes = minutes;
        }

        public Flight() { } 
    }
}
