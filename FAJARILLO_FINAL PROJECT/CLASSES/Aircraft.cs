using System;
using System.Xml.Serialization;

namespace FAJARILLO_FINAL_PROJECT.CLASSES
{
    public class Aircraft
    {
        public string Registration { get; set; }
        public string AircraftType { get; set; }
        public double MaxTakeoffWeight { get; set; }
        public double MinCg { get; set; }
        public double MaxCg { get; set; }
        public double BasicEmptyWeight { get; set; }
        public double BasicEmptyArm { get; set; }
        public double FrontSeatArm { get; set; }
        public double FuelArm { get; set; }
        public double BaggageArm { get; set; }
        public DateTime AirworthinessExpiryDate { get; set; }
        public DateTime RegistrationExpiryDate { get; set; }
        public DateTime InsuranceExpiryDate { get; set; }

        [XmlIgnore]
        public string DisplayName
        {
            get { return Registration + " - " + AircraftType; }
        }

        public Aircraft()
        {
            Registration = string.Empty;
            AircraftType = string.Empty;
            MaxTakeoffWeight = 1670;
            MinCg = 32.65;
            MaxCg = 35.00;
            BasicEmptyWeight = 1110;
            BasicEmptyArm = 32.65;
            FrontSeatArm = 39.00;
            FuelArm = 42.00;
            BaggageArm = 64.00;
            AirworthinessExpiryDate = DateTime.Today.AddMonths(6);
            RegistrationExpiryDate = DateTime.Today.AddMonths(6);
            InsuranceExpiryDate = DateTime.Today.AddMonths(6);
        }
    }
}
