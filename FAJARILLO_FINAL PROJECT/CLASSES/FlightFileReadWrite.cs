using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAJARILLO_FINAL_PROJECT.CLASSES
{
    public static class FlightFileReadWrite
    {

        public static string GetUserFlightFile(string username)
        {
            string folder = "C:\\Users\\Lee Adrian\\source\\repos\\FAJARILLO_FINAL PROJECT\\FAJARILLO FINAL PROJECT\\USER DATA\\Flights";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            return Path.Combine(folder, $"{username}_flights.txt");
        }

        public static List<Flight> ReadFlights(string username)
        {
            string file = GetUserFlightFile(username);
            var flights = new List<Flight>();

            if (!File.Exists(file)) return flights;

            using (StreamReader sr = new StreamReader(file))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 5 && DateTime.TryParse(parts[2], out DateTime date))
                    {
                        if (int.TryParse(parts[3], out int hours) &&
                            int.TryParse(parts[4], out int minutes))
                        {
                            flights.Add(new Flight(parts[0], parts[1], date, hours, minutes));
                        }
                    }
                }
            }
            return flights;
        }

        public static void SaveFlights(string username, List<Flight> flights)
        {
            string file = GetUserFlightFile(username);
            using (StreamWriter sw = new StreamWriter(file, false))
            {
                foreach (var f in flights)
                {
                    sw.WriteLine($"{f.AircraftType}|{f.Registration}|{f.FlightDate:yyyy-MM-dd}|{f.Hours}|{f.Minutes}");
                }
            }
        }
    }
}
