using FAJARILLO_FINAL_PROJECT.CLASSES;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FAJARILLO_FINAL_PROJECT.WINDOWS
{
    public partial class Dashboard : Window
    {
        private readonly Users currentUser;
        private List<Users> allUsers = new List<Users>();
        private List<Aircraft> allAircraft = new List<Aircraft>();
        private List<PreflightRecord> allRecords = new List<PreflightRecord>();
        private string editingRecordId = string.Empty;

        public Dashboard(Users user)
        {
            InitializeComponent();
            currentUser = user;
            ConfigureStaticSources();
            LoadAllData();
            ApplyRoleAccess();
            ResetFormDefaults();
        }

        private void ConfigureStaticSources()
        {
            AdminRoleComboBox.ItemsSource = Enum.GetValues(typeof(UserRole));
            RecordSortComboBox.SelectedIndex = 0;
            RecordStatusFilterComboBox.ItemsSource = new List<string> { "All Statuses", "Submitted", "Reviewed" };
            RecordStatusFilterComboBox.SelectedIndex = 0;
        }

        private void LoadAllData()
        {
            allUsers = AppRepository.LoadUsers();
            allAircraft = AppRepository.LoadAircraft();
            allRecords = AppRepository.LoadRecords();

            WelcomeText.Text = "Welcome, " + currentUser.Name;
            RoleSummaryText.Text = "Role: " + currentUser.Role + " | Access is filtered according to your permissions.";

            var students = allUsers.Where(u => u.Role == UserRole.Student).OrderBy(u => u.Name).ToList();
            var instructors = allUsers.Where(u => u.Role == UserRole.Instructor || u.Role == UserRole.Admin).OrderBy(u => u.Name).ToList();

            StudentComboBox.ItemsSource = students;
            InstructorComboBox.ItemsSource = instructors;
            AircraftComboBox.ItemsSource = allAircraft.OrderBy(a => a.Registration).ToList();

            RecordStudentFilterComboBox.ItemsSource = new List<string> { "All Students" }.Concat(students.Select(s => s.UserName)).ToList();
            RecordStudentFilterComboBox.SelectedIndex = 0;
            RecordAircraftFilterComboBox.ItemsSource = new List<string> { "All Aircraft" }.Concat(allAircraft.Select(a => a.Registration)).ToList();
            RecordAircraftFilterComboBox.SelectedIndex = 0;

            UsersListView.ItemsSource = allUsers.OrderBy(u => u.Name).ToList();
            AircraftListView.ItemsSource = allAircraft.OrderBy(a => a.Registration).ToList();

            RefreshOverview();
            RefreshRecordList();
            UpdateAircraftSummary();
            UpdateFormValidationState();
        }

        private void ApplyRoleAccess()
        {
            bool isAdmin = currentUser.Role == UserRole.Admin;
            bool canReview = currentUser.Role == UserRole.Instructor || currentUser.Role == UserRole.Admin;

            UserManagementTab.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            AircraftManagementTab.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            MarkReviewedButton.Visibility = canReview ? Visibility.Visible : Visibility.Collapsed;
            ReviewNotesTextBox.Visibility = canReview ? Visibility.Visible : Visibility.Collapsed;

            if (currentUser.Role == UserRole.Student)
            {
                StudentComboBox.SelectedValue = currentUser.UserName;
                StudentComboBox.IsEnabled = false;
            }

            if (currentUser.Role == UserRole.Instructor)
            {
                InstructorComboBox.SelectedValue = currentUser.UserName;
            }
        }

        private void RefreshOverview()
        {
            OverviewUsersText.Text = allUsers.Count.ToString();
            OverviewAircraftText.Text = allAircraft.Count.ToString();
            OverviewRecordsText.Text = allRecords.Count.ToString();
            OverviewPendingText.Text = allRecords.Count(r => r.Status == "Submitted").ToString();
        }

        private void RefreshRecordList()
        {
            IEnumerable<PreflightRecord> query = allRecords;

            if (currentUser.Role == UserRole.Student)
            {
                query = query.Where(r => r.StudentUsername == currentUser.UserName);
            }

            string studentFilter = RecordStudentFilterComboBox.SelectedItem as string;
            if (!string.IsNullOrWhiteSpace(studentFilter) && studentFilter != "All Students")
            {
                query = query.Where(r => r.StudentUsername == studentFilter);
            }

            string aircraftFilter = RecordAircraftFilterComboBox.SelectedItem as string;
            if (!string.IsNullOrWhiteSpace(aircraftFilter) && aircraftFilter != "All Aircraft")
            {
                query = query.Where(r => r.AircraftRegistration == aircraftFilter);
            }

            string statusFilter = RecordStatusFilterComboBox.SelectedItem as string;
            if (!string.IsNullOrWhiteSpace(statusFilter) && statusFilter != "All Statuses")
            {
                query = query.Where(r => r.Status == statusFilter);
            }

            if (RecordFromDatePicker.SelectedDate.HasValue)
            {
                query = query.Where(r => r.FlightDate.Date >= RecordFromDatePicker.SelectedDate.Value.Date);
            }

            if (RecordToDatePicker.SelectedDate.HasValue)
            {
                query = query.Where(r => r.FlightDate.Date <= RecordToDatePicker.SelectedDate.Value.Date);
            }

            ComboBoxItem selectedSort = RecordSortComboBox.SelectedItem as ComboBoxItem;
            string sort = selectedSort != null ? selectedSort.Content.ToString() : "Latest First";
            switch (sort)
            {
                case "Earliest First":
                    query = query.OrderBy(r => r.FlightDate).ThenBy(r => r.StudentUsername);
                    break;
                case "Student A-Z":
                    query = query.OrderBy(r => r.StudentUsername).ThenByDescending(r => r.FlightDate);
                    break;
                case "Aircraft A-Z":
                    query = query.OrderBy(r => r.AircraftRegistration).ThenByDescending(r => r.FlightDate);
                    break;
                default:
                    query = query.OrderByDescending(r => r.FlightDate).ThenBy(r => r.StudentUsername);
                    break;
            }

            List<PreflightRecord> results = query.ToList();
            RecordsListView.ItemsSource = results;
            RecordSummaryTextBlock.Text = "Showing " + results.Count + " record(s).";
        }

        private void UpdateFormValidationState()
        {
            List<string> missing = new List<string>();

            EvaluateRequiredDate(FlightDatePicker, "Date of Flight", missing);
            EvaluateRequiredText(DepartureTextBox, "Departure Aerodrome", missing);
            EvaluateRequiredText(ArrivalTextBox, "Arrival Aerodrome", missing);
            EvaluateRequiredText(LessonCodeTextBox, "Lesson Code", missing);
            EvaluateRequiredSelection(StudentComboBox, "Student", missing);
            EvaluateRequiredSelection(InstructorComboBox, "Instructor", missing);
            EvaluateRequiredSelection(AircraftComboBox, "Aircraft", missing);
            EvaluateRequiredText(HomeMetarTextBox, "Home METAR", missing);
            EvaluateRequiredText(HomeTafTextBox, "Home TAF", missing);
            EvaluateRequiredText(HomeNotamsTextBox, "Home NOTAMs", missing);
            EvaluateRequiredText(AlternateMetarTextBox, "Alternate METAR", missing);
            EvaluateRequiredText(AlternateTafTextBox, "Alternate TAF", missing);
            EvaluateRequiredText(AlternateNotamsTextBox, "Alternate NOTAMs", missing);
            EvaluateRequiredText(AirworkMetarTextBox, "Airwork METAR", missing);
            EvaluateRequiredText(AirworkTafTextBox, "Airwork TAF", missing);
            EvaluateRequiredText(AirworkNotamsTextBox, "Airwork NOTAMs", missing);
            EvaluateNumericInput(StudentWeightTextBox, "Student Weight", missing);
            EvaluateNumericInput(InstructorWeightTextBox, "Instructor Weight", missing);
            EvaluateNumericInput(FuelWeightTextBox, "Fuel Weight", missing);
            EvaluateNumericInput(BaggageWeightTextBox, "Baggage Weight", missing);

            if (missing.Count == 0)
            {
                FormValidationTextBlock.Foreground = Brushes.ForestGreen;
                FormValidationTextBlock.Text = "All required fields are complete.";
            }
            else
            {
                FormValidationTextBlock.Foreground = Brushes.Firebrick;
                FormValidationTextBlock.Text = "Missing or invalid: " + string.Join(", ", missing);
            }

            UpdateExpiryAlerts();
            UpdateWeightAndBalance();
        }

        private void EvaluateRequiredText(TextBox textBox, string label, List<string> missing)
        {
            bool valid = !string.IsNullOrWhiteSpace(textBox.Text);
            textBox.Background = valid ? Brushes.White : new SolidColorBrush(Color.FromRgb(255, 237, 237));
            if (!valid)
            {
                missing.Add(label);
            }
        }

        private void EvaluateRequiredSelection(ComboBox comboBox, string label, List<string> missing)
        {
            bool valid = comboBox.SelectedItem != null;
            comboBox.Background = valid ? Brushes.White : new SolidColorBrush(Color.FromRgb(255, 237, 237));
            if (!valid)
            {
                missing.Add(label);
            }
        }

        private void EvaluateRequiredDate(DatePicker datePicker, string label, List<string> missing)
        {
            bool valid = datePicker.SelectedDate.HasValue;
            datePicker.Background = valid ? Brushes.White : new SolidColorBrush(Color.FromRgb(255, 237, 237));
            if (!valid)
            {
                missing.Add(label);
            }
        }

        private void EvaluateNumericInput(TextBox textBox, string label, List<string> missing)
        {
            double value;
            bool valid = double.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) && value >= 0;
            textBox.Background = valid ? Brushes.White : new SolidColorBrush(Color.FromRgb(255, 237, 237));
            if (!valid)
            {
                missing.Add(label);
            }
        }

        private void UpdateAircraftSummary()
        {
            Aircraft selectedAircraft = AircraftComboBox.SelectedItem as Aircraft;
            if (selectedAircraft == null)
            {
                AircraftLimitsTextBlock.Text = "Select an aircraft to load predefined weight and CG limitations.";
                return;
            }

            AircraftLimitsTextBlock.Text =
                string.Format("MTOW: {0:F0} lbs\n", selectedAircraft.MaxTakeoffWeight) +
                string.Format("CG Envelope: {0:F2} - {1:F2}\n", selectedAircraft.MinCg, selectedAircraft.MaxCg) +
                string.Format("Basic Empty Weight: {0:F1} lbs @ {1:F2} in", selectedAircraft.BasicEmptyWeight, selectedAircraft.BasicEmptyArm);
        }

        private void UpdateWeightAndBalance()
        {
            Aircraft aircraft = AircraftComboBox.SelectedItem as Aircraft;
            if (aircraft == null)
            {
                WeightResultTextBlock.Text = "Select an aircraft to calculate weight and balance.";
                WeightStatusTextBlock.Text = string.Empty;
                return;
            }

            double studentWeight;
            double instructorWeight;
            double fuelWeight;
            double baggageWeight;

            if (!TryParseDouble(StudentWeightTextBox.Text, out studentWeight) ||
                !TryParseDouble(InstructorWeightTextBox.Text, out instructorWeight) ||
                !TryParseDouble(FuelWeightTextBox.Text, out fuelWeight) ||
                !TryParseDouble(BaggageWeightTextBox.Text, out baggageWeight))
            {
                WeightResultTextBlock.Text = "Enter numeric values for all weight inputs.";
                WeightStatusTextBlock.Text = "Weight and balance cannot be verified yet.";
                WeightStatusTextBlock.Foreground = Brushes.Firebrick;
                return;
            }

            double totalWeight = aircraft.BasicEmptyWeight + studentWeight + instructorWeight + fuelWeight + baggageWeight;
            double totalMoment =
                (aircraft.BasicEmptyWeight * aircraft.BasicEmptyArm) +
                (studentWeight * aircraft.FrontSeatArm) +
                (instructorWeight * aircraft.FrontSeatArm) +
                (fuelWeight * aircraft.FuelArm) +
                (baggageWeight * aircraft.BaggageArm);

            double cg = totalWeight <= 0 ? 0 : totalMoment / totalWeight;
            bool isWeightValid = totalWeight <= aircraft.MaxTakeoffWeight;
            bool isCgValid = cg >= aircraft.MinCg && cg <= aircraft.MaxCg;

            WeightResultTextBlock.Text =
                string.Format("Total Weight: {0:F1} lbs\n", totalWeight) +
                string.Format("Total Moment: {0:F1} in-lb\n", totalMoment) +
                string.Format("Center of Gravity: {0:F2} in", cg);

            if (isWeightValid && isCgValid)
            {
                WeightStatusTextBlock.Text = "Verified: aircraft remains within predefined weight and CG limits.";
                WeightStatusTextBlock.Foreground = Brushes.ForestGreen;
            }
            else
            {
                string statusMessage = (isWeightValid ? string.Empty : "Maximum takeoff weight exceeded. ") +
                    (isCgValid ? string.Empty : "Center of gravity is outside the permitted envelope.");
                WeightStatusTextBlock.Text = statusMessage.Trim();
                WeightStatusTextBlock.Foreground = Brushes.Firebrick;
            }
        }

        private void UpdateExpiryAlerts()
        {
            DateTime flightDate = FlightDatePicker.SelectedDate ?? DateTime.Today;
            Users student = StudentComboBox.SelectedItem as Users;
            Users instructor = InstructorComboBox.SelectedItem as Users;
            Aircraft aircraft = AircraftComboBox.SelectedItem as Aircraft;
            List<string> alerts = new List<string>();

            if (student != null)
            {
                AddExpiryAlert(alerts, "Student license", student.LicenseExpiryDate, flightDate);
                AddExpiryAlert(alerts, "Student medical", student.MedicalExpiryDate, flightDate);
            }

            if (instructor != null)
            {
                AddExpiryAlert(alerts, "Instructor license", instructor.LicenseExpiryDate, flightDate);
                AddExpiryAlert(alerts, "Instructor medical", instructor.MedicalExpiryDate, flightDate);
                AddExpiryAlert(alerts, "Instructor certification", instructor.InstructorCertificationExpiryDate, flightDate);
            }

            if (aircraft != null)
            {
                AddExpiryAlert(alerts, "Aircraft airworthiness", aircraft.AirworthinessExpiryDate, flightDate);
                AddExpiryAlert(alerts, "Aircraft registration", aircraft.RegistrationExpiryDate, flightDate);
                AddExpiryAlert(alerts, "Aircraft insurance", aircraft.InsuranceExpiryDate, flightDate);
            }

            if (alerts.Count == 0)
            {
                ExpiryAlertsTextBlock.Foreground = Brushes.ForestGreen;
                ExpiryAlertsTextBlock.Text = "No document expirations are overdue or approaching the selected flight date.";
            }
            else
            {
                ExpiryAlertsTextBlock.Foreground = Brushes.Firebrick;
                ExpiryAlertsTextBlock.Text = string.Join(Environment.NewLine, alerts);
            }
        }

        private void AddExpiryAlert(List<string> alerts, string label, DateTime expiry, DateTime flightDate)
        {
            if (expiry.Date < flightDate.Date)
            {
                alerts.Add(label + " expired on " + expiry.ToString("yyyy-MM-dd") + ".");
            }
            else if (expiry.Date <= flightDate.Date.AddDays(30))
            {
                alerts.Add(label + " expires soon on " + expiry.ToString("yyyy-MM-dd") + ".");
            }
        }

        private void ResetFormDefaults()
        {
            editingRecordId = string.Empty;
            FlightDatePicker.SelectedDate = DateTime.Today;
            LessonCodeTextBox.Text = string.Empty;
            DepartureTextBox.Text = string.Empty;
            ArrivalTextBox.Text = string.Empty;
            HomeMetarTextBox.Text = string.Empty;
            HomeTafTextBox.Text = string.Empty;
            HomeNotamsTextBox.Text = string.Empty;
            AlternateMetarTextBox.Text = string.Empty;
            AlternateTafTextBox.Text = string.Empty;
            AlternateNotamsTextBox.Text = string.Empty;
            AirworkMetarTextBox.Text = string.Empty;
            AirworkTafTextBox.Text = string.Empty;
            AirworkNotamsTextBox.Text = string.Empty;
            StudentWeightTextBox.Text = "70";
            InstructorWeightTextBox.Text = currentUser.Role == UserRole.Student ? "75" : "70";
            FuelWeightTextBox.Text = "120";
            BaggageWeightTextBox.Text = "10";

            if (StudentComboBox.IsEnabled)
            {
                StudentComboBox.SelectedIndex = StudentComboBox.Items.Count > 0 ? 0 : -1;
            }
            else
            {
                StudentComboBox.SelectedValue = currentUser.UserName;
            }

            if (currentUser.Role == UserRole.Instructor || currentUser.Role == UserRole.Admin)
            {
                InstructorComboBox.SelectedValue = currentUser.UserName;
                if (InstructorComboBox.SelectedItem == null && InstructorComboBox.Items.Count > 0)
                {
                    InstructorComboBox.SelectedIndex = 0;
                }
            }
            else
            {
                InstructorComboBox.SelectedIndex = InstructorComboBox.Items.Count > 0 ? 0 : -1;
            }

            AircraftComboBox.SelectedIndex = AircraftComboBox.Items.Count > 0 ? 0 : -1;
            UpdateFormValidationState();
        }

        private PreflightRecord BuildRecordFromForm()
        {
            Aircraft aircraft = AircraftComboBox.SelectedItem as Aircraft;
            Users student = StudentComboBox.SelectedItem as Users;
            Users instructor = InstructorComboBox.SelectedItem as Users;

            double studentWeight = double.Parse(StudentWeightTextBox.Text, CultureInfo.InvariantCulture);
            double instructorWeight = double.Parse(InstructorWeightTextBox.Text, CultureInfo.InvariantCulture);
            double fuelWeight = double.Parse(FuelWeightTextBox.Text, CultureInfo.InvariantCulture);
            double baggageWeight = double.Parse(BaggageWeightTextBox.Text, CultureInfo.InvariantCulture);
            double totalWeight = aircraft.BasicEmptyWeight + studentWeight + instructorWeight + fuelWeight + baggageWeight;
            double totalMoment =
                (aircraft.BasicEmptyWeight * aircraft.BasicEmptyArm) +
                (studentWeight * aircraft.FrontSeatArm) +
                (instructorWeight * aircraft.FrontSeatArm) +
                (fuelWeight * aircraft.FuelArm) +
                (baggageWeight * aircraft.BaggageArm);
            double centerOfGravity = totalMoment / totalWeight;

            List<string> alerts = new List<string>();
            AddExpiryAlert(alerts, "Student license", student.LicenseExpiryDate, FlightDatePicker.SelectedDate.Value);
            AddExpiryAlert(alerts, "Student medical", student.MedicalExpiryDate, FlightDatePicker.SelectedDate.Value);
            AddExpiryAlert(alerts, "Instructor license", instructor.LicenseExpiryDate, FlightDatePicker.SelectedDate.Value);
            AddExpiryAlert(alerts, "Instructor medical", instructor.MedicalExpiryDate, FlightDatePicker.SelectedDate.Value);
            AddExpiryAlert(alerts, "Instructor certification", instructor.InstructorCertificationExpiryDate, FlightDatePicker.SelectedDate.Value);
            AddExpiryAlert(alerts, "Aircraft airworthiness", aircraft.AirworthinessExpiryDate, FlightDatePicker.SelectedDate.Value);
            AddExpiryAlert(alerts, "Aircraft registration", aircraft.RegistrationExpiryDate, FlightDatePicker.SelectedDate.Value);
            AddExpiryAlert(alerts, "Aircraft insurance", aircraft.InsuranceExpiryDate, FlightDatePicker.SelectedDate.Value);

            return new PreflightRecord
            {
                Id = string.IsNullOrWhiteSpace(editingRecordId) ? Guid.NewGuid().ToString("N") : editingRecordId,
                FlightDate = FlightDatePicker.SelectedDate.Value,
                DepartureAerodrome = DepartureTextBox.Text.Trim(),
                ArrivalAerodrome = ArrivalTextBox.Text.Trim(),
                StudentUsername = student.UserName,
                InstructorUsername = instructor.UserName,
                LessonCode = LessonCodeTextBox.Text.Trim(),
                AircraftRegistration = aircraft.Registration,
                HomeMetar = HomeMetarTextBox.Text.Trim(),
                HomeTaf = HomeTafTextBox.Text.Trim(),
                HomeNotams = HomeNotamsTextBox.Text.Trim(),
                AlternateMetar = AlternateMetarTextBox.Text.Trim(),
                AlternateTaf = AlternateTafTextBox.Text.Trim(),
                AlternateNotams = AlternateNotamsTextBox.Text.Trim(),
                AirworkMetar = AirworkMetarTextBox.Text.Trim(),
                AirworkTaf = AirworkTafTextBox.Text.Trim(),
                AirworkNotams = AirworkNotamsTextBox.Text.Trim(),
                StudentWeight = studentWeight,
                InstructorWeight = instructorWeight,
                FuelWeight = fuelWeight,
                BaggageWeight = baggageWeight,
                TotalWeight = totalWeight,
                TotalMoment = totalMoment,
                CenterOfGravity = centerOfGravity,
                IsWeightValid = totalWeight <= aircraft.MaxTakeoffWeight,
                IsCgValid = centerOfGravity >= aircraft.MinCg && centerOfGravity <= aircraft.MaxCg,
                ValidationSummary = FormValidationTextBlock.Text,
                ExpiryAlerts = alerts.Count == 0 ? "No alerts." : string.Join(" ", alerts),
                Status = "Submitted",
                SubmittedAt = DateTime.Now,
                SubmittedBy = currentUser.UserName,
                ReviewNotes = string.Empty
            };
        }

        private string BuildRecordDetails(PreflightRecord record)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Date of Flight: " + record.FlightDate.ToString("yyyy-MM-dd"));
            builder.AppendLine("Departure Aerodrome: " + record.DepartureAerodrome);
            builder.AppendLine("Arrival Aerodrome: " + record.ArrivalAerodrome);
            builder.AppendLine("Student: " + record.StudentUsername);
            builder.AppendLine("Instructor: " + record.InstructorUsername);
            builder.AppendLine("Lesson Code: " + record.LessonCode);
            builder.AppendLine("Aircraft: " + record.AircraftRegistration);
            builder.AppendLine();
            builder.AppendLine("Home Weather");
            builder.AppendLine("METAR: " + record.HomeMetar);
            builder.AppendLine("TAF: " + record.HomeTaf);
            builder.AppendLine("NOTAMs: " + record.HomeNotams);
            builder.AppendLine();
            builder.AppendLine("Alternate Weather");
            builder.AppendLine("METAR: " + record.AlternateMetar);
            builder.AppendLine("TAF: " + record.AlternateTaf);
            builder.AppendLine("NOTAMs: " + record.AlternateNotams);
            builder.AppendLine();
            builder.AppendLine("Airwork Area Weather");
            builder.AppendLine("METAR: " + record.AirworkMetar);
            builder.AppendLine("TAF: " + record.AirworkTaf);
            builder.AppendLine("NOTAMs: " + record.AirworkNotams);
            builder.AppendLine();
            builder.AppendLine(string.Format("Total Weight: {0:F1} lbs", record.TotalWeight));
            builder.AppendLine(string.Format("Total Moment: {0:F1} in-lb", record.TotalMoment));
            builder.AppendLine(string.Format("Center of Gravity: {0:F2} in", record.CenterOfGravity));
            builder.AppendLine("Status: " + record.Status);
            builder.AppendLine("Validation: " + record.ValidationSummary);
            builder.AppendLine("Expiry Alerts: " + record.ExpiryAlerts);
            if (!string.IsNullOrWhiteSpace(record.ReviewNotes))
            {
                builder.AppendLine("Review Notes: " + record.ReviewNotes);
            }

            return builder.ToString();
        }

        private bool TryParseDouble(string text, out double value)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) && value >= 0;
        }

        private void FormTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFormValidationState();
        }

        private void FormSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFormValidationState();
        }

        private void AircraftComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAircraftSummary();
            UpdateFormValidationState();
        }

        private void SubmitRecord_Click(object sender, RoutedEventArgs e)
        {
            UpdateFormValidationState();

            if (FormValidationTextBlock.Text != "All required fields are complete.")
            {
                MessageBox.Show("Complete the highlighted required fields before submitting.", "Incomplete Form", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            PreflightRecord record = BuildRecordFromForm();
            if (!record.IsWeightValid || !record.IsCgValid)
            {
                MessageBox.Show("Weight and balance is outside the selected aircraft limitations. Correct the values before submitting.", "Weight and Balance Alert", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AppRepository.UpsertRecord(record);
            allRecords = AppRepository.LoadRecords();
            RefreshOverview();
            RefreshRecordList();
            ResetFormDefaults();
            MessageBox.Show("Preflight documentation record saved successfully.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClearForm_Click(object sender, RoutedEventArgs e)
        {
            ResetFormDefaults();
        }

        private void RecordFilterChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshRecordList();
        }

        private void RecordsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PreflightRecord record = RecordsListView.SelectedItem as PreflightRecord;
            if (record != null)
            {
                RecordDetailsTextBox.Text = BuildRecordDetails(record);
                ReviewNotesTextBox.Text = record.ReviewNotes;
            }
        }

        private void MarkReviewed_Click(object sender, RoutedEventArgs e)
        {
            PreflightRecord record = RecordsListView.SelectedItem as PreflightRecord;
            if (record == null)
            {
                MessageBox.Show("Select a record to review first.", "No Record Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            record.Status = "Reviewed";
            record.ReviewNotes = ReviewNotesTextBox.Text.Trim();
            AppRepository.UpsertRecord(record);
            allRecords = AppRepository.LoadRecords();
            RefreshOverview();
            RefreshRecordList();
            RecordDetailsTextBox.Text = BuildRecordDetails(record);
            MessageBox.Show("Record marked as reviewed.", "Updated", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveUser_Click(object sender, RoutedEventArgs e)
        {
            AdminUserMessageTextBlock.Text = string.Empty;

            if (string.IsNullOrWhiteSpace(AdminUserNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(AdminUsernameTextBox.Text) ||
                string.IsNullOrWhiteSpace(AdminEmailTextBox.Text) ||
                string.IsNullOrWhiteSpace(AdminPasswordTextBox.Text) ||
                AdminRoleComboBox.SelectedItem == null ||
                !AdminBirthdatePicker.SelectedDate.HasValue ||
                !AdminLicenseExpiryPicker.SelectedDate.HasValue ||
                !AdminMedicalExpiryPicker.SelectedDate.HasValue ||
                !AdminInstructorExpiryPicker.SelectedDate.HasValue)
            {
                AdminUserMessageTextBlock.Text = "Complete every user field before saving.";
                return;
            }

            Users existing = allUsers.FirstOrDefault(u => u.UserName.Equals(AdminUsernameTextBox.Text.Trim(), StringComparison.OrdinalIgnoreCase));
            if (existing == null && AppRepository.UserExists(AdminUsernameTextBox.Text.Trim(), AdminEmailTextBox.Text.Trim()))
            {
                AdminUserMessageTextBlock.Text = "That username or email already exists.";
                return;
            }

            Users user = new Users
            {
                Name = AdminUserNameTextBox.Text.Trim(),
                UserName = AdminUsernameTextBox.Text.Trim(),
                Email = AdminEmailTextBox.Text.Trim(),
                Password = AdminPasswordTextBox.Text.Trim(),
                Birthdate = AdminBirthdatePicker.SelectedDate.Value.ToString("yyyy-MM-dd"),
                Role = (UserRole)AdminRoleComboBox.SelectedItem,
                IsVerified = true,
                LicenseExpiryDate = AdminLicenseExpiryPicker.SelectedDate.Value,
                MedicalExpiryDate = AdminMedicalExpiryPicker.SelectedDate.Value,
                InstructorCertificationExpiryDate = AdminInstructorExpiryPicker.SelectedDate.Value
            };

            AppRepository.UpsertUser(user);
            LoadAllData();
            AdminUserMessageTextBlock.Foreground = Brushes.ForestGreen;
            AdminUserMessageTextBlock.Text = "User saved.";
        }

        private void UsersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Users user = UsersListView.SelectedItem as Users;
            if (user != null)
            {
                AdminUserNameTextBox.Text = user.Name;
                AdminUsernameTextBox.Text = user.UserName;
                AdminEmailTextBox.Text = user.Email;
                AdminPasswordTextBox.Text = user.Password;
                AdminBirthdatePicker.SelectedDate = user.ParsedBirthdate.HasValue ? user.ParsedBirthdate.Value : DateTime.Today.AddYears(-18);
                AdminRoleComboBox.SelectedItem = user.Role;
                AdminLicenseExpiryPicker.SelectedDate = user.LicenseExpiryDate;
                AdminMedicalExpiryPicker.SelectedDate = user.MedicalExpiryDate;
                AdminInstructorExpiryPicker.SelectedDate = user.InstructorCertificationExpiryDate;
            }
        }

        private void SaveAircraft_Click(object sender, RoutedEventArgs e)
        {
            AircraftMessageTextBlock.Text = string.Empty;
            double maxWeight;
            double minCg;
            double maxCg;
            double emptyWeight;
            double emptyArm;
            double frontSeatArm;
            double fuelArm;
            double baggageArm;

            if (string.IsNullOrWhiteSpace(AircraftRegistrationTextBox.Text) ||
                string.IsNullOrWhiteSpace(AircraftTypeTextBox.Text) ||
                !TryParseDouble(AircraftMaxWeightTextBox.Text, out maxWeight) ||
                !TryParseDouble(AircraftMinCgTextBox.Text, out minCg) ||
                !TryParseDouble(AircraftMaxCgTextBox.Text, out maxCg) ||
                !TryParseDouble(AircraftEmptyWeightTextBox.Text, out emptyWeight) ||
                !TryParseDouble(AircraftEmptyArmTextBox.Text, out emptyArm) ||
                !TryParseDouble(AircraftFrontSeatArmTextBox.Text, out frontSeatArm) ||
                !TryParseDouble(AircraftFuelArmTextBox.Text, out fuelArm) ||
                !TryParseDouble(AircraftBaggageArmTextBox.Text, out baggageArm) ||
                !AircraftAirworthinessExpiryPicker.SelectedDate.HasValue ||
                !AircraftRegistrationExpiryPicker.SelectedDate.HasValue ||
                !AircraftInsuranceExpiryPicker.SelectedDate.HasValue)
            {
                AircraftMessageTextBlock.Text = "Complete every aircraft field using valid numeric values.";
                return;
            }

            Aircraft aircraft = new Aircraft
            {
                Registration = AircraftRegistrationTextBox.Text.Trim().ToUpperInvariant(),
                AircraftType = AircraftTypeTextBox.Text.Trim(),
                MaxTakeoffWeight = maxWeight,
                MinCg = minCg,
                MaxCg = maxCg,
                BasicEmptyWeight = emptyWeight,
                BasicEmptyArm = emptyArm,
                FrontSeatArm = frontSeatArm,
                FuelArm = fuelArm,
                BaggageArm = baggageArm,
                AirworthinessExpiryDate = AircraftAirworthinessExpiryPicker.SelectedDate.Value,
                RegistrationExpiryDate = AircraftRegistrationExpiryPicker.SelectedDate.Value,
                InsuranceExpiryDate = AircraftInsuranceExpiryPicker.SelectedDate.Value
            };

            AppRepository.UpsertAircraft(aircraft);
            LoadAllData();
            AircraftMessageTextBlock.Foreground = Brushes.ForestGreen;
            AircraftMessageTextBlock.Text = "Aircraft saved.";
        }

        private void AircraftListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Aircraft aircraft = AircraftListView.SelectedItem as Aircraft;
            if (aircraft != null)
            {
                AircraftRegistrationTextBox.Text = aircraft.Registration;
                AircraftTypeTextBox.Text = aircraft.AircraftType;
                AircraftMaxWeightTextBox.Text = aircraft.MaxTakeoffWeight.ToString(CultureInfo.InvariantCulture);
                AircraftMinCgTextBox.Text = aircraft.MinCg.ToString(CultureInfo.InvariantCulture);
                AircraftMaxCgTextBox.Text = aircraft.MaxCg.ToString(CultureInfo.InvariantCulture);
                AircraftEmptyWeightTextBox.Text = aircraft.BasicEmptyWeight.ToString(CultureInfo.InvariantCulture);
                AircraftEmptyArmTextBox.Text = aircraft.BasicEmptyArm.ToString(CultureInfo.InvariantCulture);
                AircraftFrontSeatArmTextBox.Text = aircraft.FrontSeatArm.ToString(CultureInfo.InvariantCulture);
                AircraftFuelArmTextBox.Text = aircraft.FuelArm.ToString(CultureInfo.InvariantCulture);
                AircraftBaggageArmTextBox.Text = aircraft.BaggageArm.ToString(CultureInfo.InvariantCulture);
                AircraftAirworthinessExpiryPicker.SelectedDate = aircraft.AirworthinessExpiryDate;
                AircraftRegistrationExpiryPicker.SelectedDate = aircraft.RegistrationExpiryDate;
                AircraftInsuranceExpiryPicker.SelectedDate = aircraft.InsuranceExpiryDate;
            }
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
    }
}
