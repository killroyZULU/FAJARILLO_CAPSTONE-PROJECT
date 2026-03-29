using System;
using System.Xml.Serialization;

namespace FAJARILLO_FINAL_PROJECT.CLASSES
{
    public enum UserRole
    {
        Student,
        Instructor,
        Admin
    }

    public class Users
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Birthdate { get; set; }
        public bool IsVerified { get; set; }
        public UserRole Role { get; set; }
        public DateTime LicenseExpiryDate { get; set; }
        public DateTime MedicalExpiryDate { get; set; }
        public DateTime InstructorCertificationExpiryDate { get; set; }

        [XmlIgnore]
        public DateTime? ParsedBirthdate
        {
            get
            {
                DateTime birthdate;
                if (DateTime.TryParse(Birthdate, out birthdate))
                {
                    return birthdate;
                }

                return null;
            }
        }

        public Users()
        {
            Birthdate = string.Empty;
            IsVerified = true;
            Role = UserRole.Student;
            LicenseExpiryDate = DateTime.Today.AddYears(1);
            MedicalExpiryDate = DateTime.Today.AddYears(1);
            InstructorCertificationExpiryDate = DateTime.Today.AddYears(1);
        }

        public Users(string name, string username, string email, string password, string birthdate, bool isVerified = false)
            : this()
        {
            Name = name;
            UserName = username;
            Email = email;
            Password = password;
            Birthdate = birthdate;
            IsVerified = isVerified;
        }
    }
}
