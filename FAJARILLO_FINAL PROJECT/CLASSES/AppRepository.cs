using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace FAJARILLO_FINAL_PROJECT.CLASSES
{
    public static class AppRepository
    {
        private static readonly string DataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "USER DATA", "DATABASE");
        private static readonly string UsersFile = Path.Combine(DataFolder, "users.xml");
        private static readonly string AircraftFile = Path.Combine(DataFolder, "aircraft.xml");
        private static readonly string RecordsFile = Path.Combine(DataFolder, "preflight-records.xml");

        public static void EnsureSeedData()
        {
            Directory.CreateDirectory(DataFolder);

            var users = LoadUsers();
            if (!users.Any())
            {
                users = new List<Users>
                {
                    new Users
                    {
                        Name = "System Administrator",
                        UserName = "admin",
                        Email = "admin@fajarillo.local",
                        Password = "admin123",
                        Birthdate = DateTime.Today.AddYears(-30).ToString("yyyy-MM-dd"),
                        Role = UserRole.Admin,
                        IsVerified = true,
                        LicenseExpiryDate = DateTime.Today.AddYears(1),
                        MedicalExpiryDate = DateTime.Today.AddYears(1),
                        InstructorCertificationExpiryDate = DateTime.Today.AddYears(1)
                    },
                    new Users
                    {
                        Name = "Lead Instructor",
                        UserName = "instructor1",
                        Email = "instructor1@fajarillo.local",
                        Password = "instructor123",
                        Birthdate = DateTime.Today.AddYears(-32).ToString("yyyy-MM-dd"),
                        Role = UserRole.Instructor,
                        IsVerified = true,
                        LicenseExpiryDate = DateTime.Today.AddMonths(9),
                        MedicalExpiryDate = DateTime.Today.AddMonths(6),
                        InstructorCertificationExpiryDate = DateTime.Today.AddMonths(8)
                    },
                    new Users
                    {
                        Name = "Student Pilot",
                        UserName = "student1",
                        Email = "student1@fajarillo.local",
                        Password = "student123",
                        Birthdate = DateTime.Today.AddYears(-20).ToString("yyyy-MM-dd"),
                        Role = UserRole.Student,
                        IsVerified = true,
                        LicenseExpiryDate = DateTime.Today.AddMonths(5),
                        MedicalExpiryDate = DateTime.Today.AddMonths(4),
                        InstructorCertificationExpiryDate = DateTime.Today.AddYears(1)
                    }
                };

                SaveUsers(users);
            }

            var aircraft = LoadAircraft();
            if (!aircraft.Any())
            {
                aircraft = new List<Aircraft>
                {
                    new Aircraft
                    {
                        Registration = "RPC-1521",
                        AircraftType = "Cessna 152",
                        MaxTakeoffWeight = 1670,
                        MinCg = 32.65,
                        MaxCg = 35.00,
                        BasicEmptyWeight = 1110,
                        BasicEmptyArm = 32.65,
                        FrontSeatArm = 39.00,
                        FuelArm = 42.00,
                        BaggageArm = 64.00,
                        AirworthinessExpiryDate = DateTime.Today.AddMonths(11),
                        RegistrationExpiryDate = DateTime.Today.AddMonths(7),
                        InsuranceExpiryDate = DateTime.Today.AddMonths(3)
                    }
                };

                SaveAircraft(aircraft);
            }

            if (!File.Exists(RecordsFile))
            {
                SaveRecords(new List<PreflightRecord>());
            }
        }

        public static List<Users> LoadUsers()
        {
            return LoadList<Users>(UsersFile);
        }

        public static void SaveUsers(List<Users> users)
        {
            SaveList(UsersFile, users);
        }

        public static List<Aircraft> LoadAircraft()
        {
            return LoadList<Aircraft>(AircraftFile);
        }

        public static void SaveAircraft(List<Aircraft> aircraft)
        {
            SaveList(AircraftFile, aircraft);
        }

        public static List<PreflightRecord> LoadRecords()
        {
            return LoadList<PreflightRecord>(RecordsFile);
        }

        public static void SaveRecords(List<PreflightRecord> records)
        {
            SaveList(RecordsFile, records.OrderByDescending(r => r.FlightDate).ToList());
        }

        public static Users Authenticate(string login, string password)
        {
            return LoadUsers().FirstOrDefault(u =>
                (string.Equals(u.UserName, login, StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(u.Email, login, StringComparison.OrdinalIgnoreCase)) &&
                u.Password == password);
        }

        public static bool UserExists(string username, string email)
        {
            return LoadUsers().Any(u =>
                string.Equals(u.UserName, username, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        }

        public static Users GetUserByUsername(string username)
        {
            return LoadUsers().FirstOrDefault(u => string.Equals(u.UserName, username, StringComparison.OrdinalIgnoreCase));
        }

        public static void UpsertUser(Users user)
        {
            var users = LoadUsers();
            var existing = users.FirstOrDefault(u => string.Equals(u.UserName, user.UserName, StringComparison.OrdinalIgnoreCase));
            if (existing == null)
            {
                users.Add(user);
            }
            else
            {
                existing.Name = user.Name;
                existing.Email = user.Email;
                existing.Password = user.Password;
                existing.Birthdate = user.Birthdate;
                existing.Role = user.Role;
                existing.IsVerified = user.IsVerified;
                existing.LicenseExpiryDate = user.LicenseExpiryDate;
                existing.MedicalExpiryDate = user.MedicalExpiryDate;
                existing.InstructorCertificationExpiryDate = user.InstructorCertificationExpiryDate;
            }

            SaveUsers(users.OrderBy(u => u.Name).ToList());
        }

        public static void UpsertAircraft(Aircraft aircraft)
        {
            var aircraftList = LoadAircraft();
            var existing = aircraftList.FirstOrDefault(a => string.Equals(a.Registration, aircraft.Registration, StringComparison.OrdinalIgnoreCase));
            if (existing == null)
            {
                aircraftList.Add(aircraft);
            }
            else
            {
                existing.AircraftType = aircraft.AircraftType;
                existing.MaxTakeoffWeight = aircraft.MaxTakeoffWeight;
                existing.MinCg = aircraft.MinCg;
                existing.MaxCg = aircraft.MaxCg;
                existing.BasicEmptyWeight = aircraft.BasicEmptyWeight;
                existing.BasicEmptyArm = aircraft.BasicEmptyArm;
                existing.FrontSeatArm = aircraft.FrontSeatArm;
                existing.FuelArm = aircraft.FuelArm;
                existing.BaggageArm = aircraft.BaggageArm;
                existing.AirworthinessExpiryDate = aircraft.AirworthinessExpiryDate;
                existing.RegistrationExpiryDate = aircraft.RegistrationExpiryDate;
                existing.InsuranceExpiryDate = aircraft.InsuranceExpiryDate;
            }

            SaveAircraft(aircraftList.OrderBy(a => a.Registration).ToList());
        }

        public static void UpsertRecord(PreflightRecord record)
        {
            var records = LoadRecords();
            var existing = records.FirstOrDefault(r => r.Id == record.Id);
            if (existing == null)
            {
                records.Add(record);
            }
            else
            {
                int index = records.IndexOf(existing);
                records[index] = record;
            }

            SaveRecords(records);
        }

        private static List<T> LoadList<T>(string path)
        {
            Directory.CreateDirectory(DataFolder);

            if (!File.Exists(path))
            {
                return new List<T>();
            }

            try
            {
                using (var stream = File.OpenRead(path))
                {
                    var serializer = new XmlSerializer(typeof(List<T>));
                    return (List<T>)serializer.Deserialize(stream);
                }
            }
            catch
            {
                return new List<T>();
            }
        }

        private static void SaveList<T>(string path, List<T> items)
        {
            Directory.CreateDirectory(DataFolder);
            using (var stream = File.Create(path))
            {
                var serializer = new XmlSerializer(typeof(List<T>));
                serializer.Serialize(stream, items);
            }
        }
    }
}
