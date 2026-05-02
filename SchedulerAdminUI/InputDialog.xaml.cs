using System.Windows;

namespace SchedulerAdminUI
{
    public partial class InputDialog : Window
    {
        public string PromptText { get; set; }
        public string InputValue { get; set; }

        public InputDialog(string title, string promptText, string defaultValue = "")
        {
            InitializeComponent();

            Title = title;
            PromptText = promptText;
            InputValue = defaultValue;

            DataContext = this;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
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