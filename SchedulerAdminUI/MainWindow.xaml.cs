using CommunityToolkit.Mvvm.Messaging;
using SchedulerAdminUI.Services;
using SchedulerAdminUI.ViewModels;
using System;
using System.Windows;
using System.Windows.Media;

namespace SchedulerAdminUI
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private int _catClickCount = 0;
        private readonly MediaPlayer _meowPlayer = new();

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

        private async void CheckUpdatesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var updater = new AppUpdateService();

                MessageBox.Show(
                    "Checking GitHub for updates...",
                    "Updater",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                var result = await updater.CheckAndUpdateAsync();

                MessageBox.Show(
                    result,
                    "Updater",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Update failed:\n{ex.Message}",
                    "Updater",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void CatButton_Click(object sender, RoutedEventArgs e)
        {
            _catClickCount++;

            CatText.Text = "🐈";

            var originalMargin = CatText.Margin;

            CatText.Margin = new Thickness(8, -18, 0, 0);
            await Task.Delay(120);
            CatText.Margin = originalMargin;

            if (_catClickCount >= 5)
            {
                _catClickCount = 0;

                try
                {
                    var path = System.IO.Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Assets",
                        "meow.mp3");

                    if (!System.IO.File.Exists(path))
                    {
                        MessageBox.Show($"Meow file not found:\n{path}");
                        return;
                    }

                    _meowPlayer.Open(new Uri(path, UriKind.Absolute));
                    _meowPlayer.Volume = 1.0;
                    _meowPlayer.Play();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Meow failed:\n{ex.Message}");
                }

                CatText.Text = "Meow!";
                await Task.Delay(1200);
                CatText.Text = "";
            }
        }
    }
}