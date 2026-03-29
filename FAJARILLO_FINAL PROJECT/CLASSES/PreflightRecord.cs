using System;
using System.Xml.Serialization;

namespace FAJARILLO_FINAL_PROJECT.CLASSES
{
    public class PreflightRecord
    {
        public string Id { get; set; }
        public DateTime FlightDate { get; set; }
        public string DepartureAerodrome { get; set; }
        public string ArrivalAerodrome { get; set; }
        public string StudentUsername { get; set; }
        public string InstructorUsername { get; set; }
        public string LessonCode { get; set; }
        public string AircraftRegistration { get; set; }
        public string HomeMetar { get; set; }
        public string HomeTaf { get; set; }
        public string HomeNotams { get; set; }
        public string AlternateMetar { get; set; }
        public string AlternateTaf { get; set; }
        public string AlternateNotams { get; set; }
        public string AirworkMetar { get; set; }
        public string AirworkTaf { get; set; }
        public string AirworkNotams { get; set; }
        public double StudentWeight { get; set; }
        public double InstructorWeight { get; set; }
        public double FuelWeight { get; set; }
        public double BaggageWeight { get; set; }
        public double TotalWeight { get; set; }
        public double TotalMoment { get; set; }
        public double CenterOfGravity { get; set; }
        public bool IsWeightValid { get; set; }
        public bool IsCgValid { get; set; }
        public string ValidationSummary { get; set; }
        public string ExpiryAlerts { get; set; }
        public string Status { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string SubmittedBy { get; set; }
        public string ReviewNotes { get; set; }

        [XmlIgnore]
        public string FlightDateDisplay
        {
            get { return FlightDate.ToString("yyyy-MM-dd"); }
        }

        [XmlIgnore]
        public string WeightBalanceDisplay
        {
            get { return string.Format("{0:F1} lbs / CG {1:F2}", TotalWeight, CenterOfGravity); }
        }

        public PreflightRecord()
        {
            Id = Guid.NewGuid().ToString("N");
            FlightDate = DateTime.Today;
            DepartureAerodrome = string.Empty;
            ArrivalAerodrome = string.Empty;
            StudentUsername = string.Empty;
            InstructorUsername = string.Empty;
            LessonCode = string.Empty;
            AircraftRegistration = string.Empty;
            HomeMetar = string.Empty;
            HomeTaf = string.Empty;
            HomeNotams = string.Empty;
            AlternateMetar = string.Empty;
            AlternateTaf = string.Empty;
            AlternateNotams = string.Empty;
            AirworkMetar = string.Empty;
            AirworkTaf = string.Empty;
            AirworkNotams = string.Empty;
            ValidationSummary = string.Empty;
            ExpiryAlerts = string.Empty;
            Status = "Submitted";
            SubmittedAt = DateTime.Now;
            SubmittedBy = string.Empty;
            ReviewNotes = string.Empty;
        }
    }
}
