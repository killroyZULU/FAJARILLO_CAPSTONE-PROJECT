using FAJARILLO_FINAL_PROJECT.CLASSES;
using FAJARILLO_FINAL_PROJECT.WINDOWS;
using System;
using System.Windows;

namespace FAJARILLO_FINAL_PROJECT
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BirthdatePicker.SelectedDate = DateTime.Today.AddYears(-18);
            LicenseExpiryPicker.SelectedDate = DateTime.Today.AddMonths(6);
            MedicalExpiryPicker.SelectedDate = DateTime.Today.AddMonths(6);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            LoginMessageTextBlock.Text = string.Empty;

            string login = LoginIdentityTextBox.Text.Trim();
            string password = LoginPasswordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                LoginMessageTextBlock.Text = "Enter both your username/email and password.";
                return;
            }

            Users user = AppRepository.Authenticate(login, password);
            if (user == null)
            {
                LoginMessageTextBlock.Text = "Invalid credentials. Use one of the seeded demo accounts or create a student account.";
                return;
            }

            Dashboard dashboard = new Dashboard(user);
            dashboard.Show();
            Close();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            SignUpMessageTextBlock.Text = string.Empty;

            string name = SignUpNameTextBox.Text.Trim();
            string username = SignUpUsernameTextBox.Text.Trim();
            string email = SignUpEmailTextBox.Text.Trim();
            string password = SignUpPasswordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                !BirthdatePicker.SelectedDate.HasValue ||
                !LicenseExpiryPicker.SelectedDate.HasValue ||
                !MedicalExpiryPicker.SelectedDate.HasValue)
            {
                SignUpMessageTextBlock.Text = "Complete every required field before creating the account.";
                return;
            }

            if (AppRepository.UserExists(username, email))
            {
                SignUpMessageTextBlock.Text = "That username or email is already registered.";
                return;
            }

            Users user = new Users
            {
                Name = name,
                UserName = username,
                Email = email,
                Password = password,
                Birthdate = BirthdatePicker.SelectedDate.Value.ToString("yyyy-MM-dd"),
                Role = UserRole.Student,
                IsVerified = true,
                LicenseExpiryDate = LicenseExpiryPicker.SelectedDate.Value,
                MedicalExpiryDate = MedicalExpiryPicker.SelectedDate.Value,
                InstructorCertificationExpiryDate = DateTime.Today.AddYears(1)
            };

            AppRepository.UpsertUser(user);

            Dashboard dashboard = new Dashboard(user);
            dashboard.Show();
            Close();
        }

        private void ShowSignUp_Click(object sender, RoutedEventArgs e)
        {
            LoginPanel.Visibility = Visibility.Collapsed;
            SignUpPanel.Visibility = Visibility.Visible;
        }

        private void ShowLogin_Click(object sender, RoutedEventArgs e)
        {
            SignUpPanel.Visibility = Visibility.Collapsed;
            LoginPanel.Visibility = Visibility.Visible;
        }
    }
}
