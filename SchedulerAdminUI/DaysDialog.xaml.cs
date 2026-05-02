using System.Windows;

namespace SchedulerAdminUI
{
    public partial class DaysDialog : Window
    {
        public List<string> SelectedDays { get; private set; } = new();

        public DaysDialog(List<string> currentDays)
        {
            InitializeComponent();

            currentDays ??= new List<string>();

            MondayCheckBox.IsChecked = currentDays.Contains("Monday", StringComparer.OrdinalIgnoreCase);
            TuesdayCheckBox.IsChecked = currentDays.Contains("Tuesday", StringComparer.OrdinalIgnoreCase);
            WednesdayCheckBox.IsChecked = currentDays.Contains("Wednesday", StringComparer.OrdinalIgnoreCase);
            ThursdayCheckBox.IsChecked = currentDays.Contains("Thursday", StringComparer.OrdinalIgnoreCase);
            FridayCheckBox.IsChecked = currentDays.Contains("Friday", StringComparer.OrdinalIgnoreCase);
            SaturdayCheckBox.IsChecked = currentDays.Contains("Saturday", StringComparer.OrdinalIgnoreCase);
            SundayCheckBox.IsChecked = currentDays.Contains("Sunday", StringComparer.OrdinalIgnoreCase);
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            SelectedDays.Clear();

            if (MondayCheckBox.IsChecked == true) SelectedDays.Add("Monday");
            if (TuesdayCheckBox.IsChecked == true) SelectedDays.Add("Tuesday");
            if (WednesdayCheckBox.IsChecked == true) SelectedDays.Add("Wednesday");
            if (ThursdayCheckBox.IsChecked == true) SelectedDays.Add("Thursday");
            if (FridayCheckBox.IsChecked == true) SelectedDays.Add("Friday");
            if (SaturdayCheckBox.IsChecked == true) SelectedDays.Add("Saturday");
            if (SundayCheckBox.IsChecked == true) SelectedDays.Add("Sunday");

            if (SelectedDays.Count == 0)
            {
                MessageBox.Show("Please select at least one day.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}