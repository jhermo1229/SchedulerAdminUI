using CommunityToolkit.Mvvm.Messaging;
using SchedulerAdminUI.ViewModels;
using System;
using System.Windows;

namespace SchedulerAdminUI
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadAsync();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadAsync();
        }

        private async void RunNowButton_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.RunSelectedJobAsync();
        }

        private async void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.ToggleSelectedJobAsync();
        }

        private async void AddRecipientButton_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.AddRecipientToSelectedJobAsync();
        }

        private async void RemoveRecipientButton_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.RemoveSelectedRecipientAsync();
        }

        private async void ApiUrlComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            await _viewModel.ChangeApiUrlAndReloadAsync();
        }

        private async void ChangeTimeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedJob == null)
                return;

            var dialog = new InputDialog(
                "Change Time",
                $"Enter new time for '{_viewModel.SelectedJob.Name}' in HH:mm format:",
                _viewModel.SelectedJob.TimeOfDay)
            {
                Owner = this
            };

            if (dialog.ShowDialog() != true)
                return;

            await _viewModel.ChangeSelectedJobTimeAsync(dialog.InputValue);
        }

        private async void EditDaysButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedJob == null)
                return;

            var dialog = new DaysDialog(_viewModel.SelectedJob.DaysOfWeek.ToList())
            {
                Owner = this
            };

            if (dialog.ShowDialog() != true)
                return;

            await _viewModel.ChangeSelectedJobDaysAsync(dialog.SelectedDays);
        }

        private async void RenameJobButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedJob == null)
                return;

            var dialog = new InputDialog(
                "Rename Job",
                $"Enter new name for '{_viewModel.SelectedJob.Name}':",
                _viewModel.SelectedJob.Name)
            {
                Owner = this
            };

            if (dialog.ShowDialog() != true)
                return;

            await _viewModel.RenameSelectedJobAsync(dialog.InputValue);
        }
    }
}