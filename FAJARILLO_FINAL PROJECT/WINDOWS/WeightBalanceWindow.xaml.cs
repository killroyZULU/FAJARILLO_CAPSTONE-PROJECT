using FAJARILLO_FINAL_PROJECT.CLASSES;
using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Interaction logic for WeightBalanceWindow.xaml
    /// </summary>
    public partial class WeightBalanceWindow : Window
    {
        private Users currentUser;

        public WeightBalanceWindow(Users user)
        {
            InitializeComponent();
            currentUser = user;
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // --- Get all weights (use 0 if empty) ---
                double emptyWeight = (EmptyWeightInput.Text != "") ? double.Parse(EmptyWeightInput.Text) : 0;
                double studentWeight = (StudentWeightInput.Text != "") ? double.Parse(StudentWeightInput.Text) : 0;
                double instructorWeight = (InstructorWeightInput.Text != "") ? double.Parse(InstructorWeightInput.Text) : 0;
                double fuelWeight = (FuelWeightInput.Text != "") ? double.Parse(FuelWeightInput.Text) : 0;
                double baggageWeight = (BaggageWeightInput.Text != "") ? double.Parse(BaggageWeightInput.Text) : 0;

                // --- Get all arms ---
                double emptyArm = double.Parse(EmptyWeightArmInput.Text);
                double studentArm = double.Parse(StudentArmInput.Text);
                double instructorArm = double.Parse(InstructorArmInput.Text);
                double fuelArm = double.Parse(FuelArmInput.Text);
                double baggageArm = double.Parse(BaggageArmInput.Text);

                // --- Calculate totals ---
                double totalWeight = emptyWeight + studentWeight + instructorWeight + fuelWeight + baggageWeight;

                double totalMoment =
                    (emptyWeight * emptyArm) +
                    (studentWeight * studentArm) +
                    (instructorWeight * instructorArm) +
                    (fuelWeight * fuelArm) +
                    (baggageWeight * baggageArm);

                double cg = totalMoment / totalWeight;

                // --- Display results ---
                ResultTotalWeight.Text = "Total Weight: " + totalWeight + " lbs";
                ResultTotalMoment.Text = "Total Moment: " + totalMoment + " in-lb";
                ResultCG.Text = "Center of Gravity: " + cg + " in";

                // --- Check limits ---
                if (totalWeight > 1670)
                {
                    ResultStatus.Text = "❌ Aircraft is OVERWEIGHT!";
                    ResultStatus.Foreground = Brushes.Red;
                }
                else if (cg < 32.65 || cg > 35.0)
                {
                    ResultStatus.Text = "❌ CG OUT OF LIMITS!";
                    ResultStatus.Foreground = Brushes.Red;
                }
                else
                {
                    ResultStatus.Text = "✅ Aircraft is WITHIN LIMITS";
                    ResultStatus.Foreground = Brushes.Green;
                }
            }
            catch
            {
                MessageBox.Show("Please enter valid numbers.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Dashboard dashboard = new Dashboard(currentUser);
            dashboard.Show();
            this.Close();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Calculate_Click(null, null);
            }
        }
    }
}
