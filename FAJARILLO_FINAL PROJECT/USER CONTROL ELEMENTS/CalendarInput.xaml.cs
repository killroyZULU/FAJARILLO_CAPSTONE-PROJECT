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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FAJARILLO_FINAL_PROJECT.USER_CONTROL_ELEMENTS
{
    /// <summary>
    /// Interaction logic for CalendarInput.xaml
    /// </summary>
    public partial class CalendarInput : UserControl
    {
        public DateTime? SelectedDate { get; private set; }


        public CalendarInput()
        {
            InitializeComponent();
        }

        private string placeholder;

        public string Placeholder
        {
            get { return placeholder; }
            set
            {
                placeholder = value;

                // Apply placeholder to the TextBox
                if (DateTextBox != null) // replace with your actual TextBox name
                {
                    PlaceholderText.Text = placeholder;
                    PlaceholderText.Foreground = Brushes.Gray;
                }
            }
        }
        public string Text
        {
            get => SelectedDate?.ToString("yyyy-MM-dd") ?? string.Empty;
        }

        private void OpenCalendarButton_Click(object sender, RoutedEventArgs e)
        {
            CalendarPopup.IsOpen = true;
        }

        private void CalendarControl_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CalendarControl.SelectedDate != null)
            {
                SelectedDate = CalendarControl.SelectedDate;
                DateTextBox.Text = SelectedDate?.ToString("MMMM dd, yyyy");
                PlaceholderText.Visibility = Visibility.Collapsed;
                CalendarPopup.IsOpen = false;
            }
            else
            {
                PlaceholderText.Visibility = Visibility.Visible;
            }
        }
        public void Clear()
        {
            SelectedDate = null;
            DateTextBox.Text = string.Empty;
            PlaceholderText.Visibility = Visibility.Visible;
        }
    }
}
