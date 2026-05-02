using CommunityToolkit.Mvvm.ComponentModel;
using SchedulerAdminUI.Models;
using SchedulerAdminUI.Services;
using System.Collections.ObjectModel;
using System.Net.Http;

namespace SchedulerAdminUI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly SchedulerApiService _apiService;

        [ObservableProperty]
        private string schedulerMessage = "Not loaded";

        [ObservableProperty]
        private bool isSchedulerRunning;

        [ObservableProperty]
        private int configuredJobCount;

        [ObservableProperty]
        private SchedulerJobDto? selectedJob;

        [ObservableProperty]
        private string errorMessage = "";

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string newRecipientEmail = "";

        [ObservableProperty]
        private string? selectedRecipient;

        [ObservableProperty]
        private string loadingMessage = "";

        [ObservableProperty]
        private string? selectedApiUrl;

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        public ObservableCollection<SchedulerJobDto> Jobs { get; } = new();

        public ObservableCollection<string> ApiUrls { get; } = new()
        {            
            "http://192.168.2.109:5233/",
            "http://localhost:5233/"
        };

        public bool CanEditActions => !IsLoading && SelectedJob != null;
        public bool CanRemoveRecipient => !IsLoading && SelectedJob != null && !string.IsNullOrWhiteSpace(SelectedRecipient);

        public MainViewModel()
        {
            SelectedApiUrl = ApiUrls[0];
            _apiService = new SchedulerApiService(SelectedApiUrl);
        }

        partial void OnErrorMessageChanged(string value)
        {
            OnPropertyChanged(nameof(HasError));
        }

        partial void OnIsLoadingChanged(bool value)
        {
            OnPropertyChanged(nameof(CanEditActions));
            OnPropertyChanged(nameof(CanRemoveRecipient));
        }

        partial void OnSelectedJobChanged(SchedulerJobDto? value)
        {
            OnPropertyChanged(nameof(CanEditActions));
            OnPropertyChanged(nameof(CanRemoveRecipient));
        }

        partial void OnSelectedRecipientChanged(string? value)
        {
            OnPropertyChanged(nameof(CanRemoveRecipient));
        }

        public async Task ChangeApiUrlAndReloadAsync()
        {
            if (string.IsNullOrWhiteSpace(SelectedApiUrl))
                return;

            _apiService.SetBaseUrl(SelectedApiUrl);
            await LoadAsync();
        }

        public async Task LoadAsync()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Loading scheduler data...";
                ErrorMessage = "";

                var previousSelectedJobName = SelectedJob?.Name;

                var status = await _apiService.GetStatusAsync();
                if (status != null)
                {
                    IsSchedulerRunning = status.IsRunning;
                    ConfiguredJobCount = status.ConfiguredJobCount;
                    SchedulerMessage = status.LastMessage ?? "No message";
                }

                var jobs = await _apiService.GetJobsAsync();

                Jobs.Clear();
                foreach (var job in jobs)
                {
                    Jobs.Add(job);
                }

                if (!string.IsNullOrWhiteSpace(previousSelectedJobName))
                    SelectedJob = Jobs.FirstOrDefault(j => j.Name == previousSelectedJobName);

                if (SelectedJob == null && Jobs.Count > 0)
                    SelectedJob = Jobs[0];

                SelectedRecipient = null;
            }
            catch (HttpRequestException)
            {
                ErrorMessage =
                    "Unable to connect to Scheduler API.\n\n" +
                    "Possible reasons:\n" +
                    "• Scheduler laptop is turned off\n" +
                    "• API service is not running\n" +
                    "• Wrong API server selected\n" +
                    "• Network or VPN unavailable\n\n" +
                    "Please verify the IT Room laptop is online.";
            }
            catch (TaskCanceledException)
            {
                ErrorMessage =
                    "Connection timed out while contacting Scheduler API.\n\n" +
                    "The scheduler machine may be offline or unreachable.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Unexpected error:\n{ex.Message}";
            }
            finally
            {
                LoadingMessage = "";
                IsLoading = false;
            }
        }

        public async Task RunSelectedJobAsync()
        {
            if (SelectedJob == null)
                return;

            try
            {
                IsLoading = true;
                LoadingMessage = $"Running {SelectedJob.Name}...";
                ErrorMessage = "";

                var selectedJobName = SelectedJob.Name;

                var success = await _apiService.RunJobNowAsync(selectedJobName);

                if (!success)
                    ErrorMessage = "Failed to run job.";
                else
                    SchedulerMessage = $"Ran job: {selectedJobName}";

                await LoadAsync();
                SelectedJob = Jobs.FirstOrDefault(j => j.Name == selectedJobName);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                LoadingMessage = "";
                IsLoading = false;
            }
        }

        public async Task ToggleSelectedJobAsync()
        {
            if (SelectedJob == null)
                return;

            try
            {
                IsLoading = true;
                LoadingMessage = $"Updating {SelectedJob.Name}...";
                ErrorMessage = "";

                var selectedJobName = SelectedJob.Name;
                var newValue = !SelectedJob.Enabled;

                var success = await _apiService.UpdateJobEnabledAsync(selectedJobName, newValue);

                if (!success)
                {
                    ErrorMessage = "Failed to update job.";
                    return;
                }

                await LoadAsync();
                SelectedJob = Jobs.FirstOrDefault(j => j.Name == selectedJobName);

                SchedulerMessage = $"Updated job: {selectedJobName}";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                LoadingMessage = "";
                IsLoading = false;
            }
        }

        public async Task AddRecipientToSelectedJobAsync()
        {
            if (SelectedJob == null)
                return;

            if (string.IsNullOrWhiteSpace(NewRecipientEmail))
            {
                ErrorMessage = "Enter an email address first.";
                return;
            }

            try
            {
                IsLoading = true;
                LoadingMessage = "Adding recipient...";
                ErrorMessage = "";

                var selectedJobName = SelectedJob.Name;
                var emailToAdd = NewRecipientEmail.Trim();

                var success = await _apiService.AddRecipientAsync(selectedJobName, emailToAdd);

                if (!success)
                {
                    ErrorMessage = "Failed to add recipient.";
                    return;
                }

                NewRecipientEmail = "";
                await LoadAsync();
                SelectedJob = Jobs.FirstOrDefault(j => j.Name == selectedJobName);

                SchedulerMessage = $"Added recipient to {selectedJobName}";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                LoadingMessage = "";
                IsLoading = false;
            }
        }

        public async Task RemoveSelectedRecipientAsync()
        {
            if (SelectedJob == null)
                return;

            if (string.IsNullOrWhiteSpace(SelectedRecipient))
            {
                ErrorMessage = "Select a recipient first.";
                return;
            }

            try
            {
                IsLoading = true;
                LoadingMessage = "Removing recipient...";
                ErrorMessage = "";

                var selectedJobName = SelectedJob.Name;
                var emailToRemove = SelectedRecipient;

                var success = await _apiService.RemoveRecipientAsync(selectedJobName, emailToRemove);

                if (!success)
                {
                    ErrorMessage = "Failed to remove recipient.";
                    return;
                }

                await LoadAsync();
                SelectedJob = Jobs.FirstOrDefault(j => j.Name == selectedJobName);
                SelectedRecipient = null;

                SchedulerMessage = $"Removed recipient from {selectedJobName}";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                LoadingMessage = "";
                IsLoading = false;
            }
        }

        public async Task ChangeSelectedJobTimeAsync(string newTime)
        {
            if (SelectedJob == null)
                return;

            if (string.IsNullOrWhiteSpace(newTime))
            {
                ErrorMessage = "Time cannot be empty.";
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(newTime.Trim(), @"^([01]\d|2[0-3]):[0-5]\d$"))
            {
                ErrorMessage = "Time must be in HH:mm format, example: 08:00 or 15:30.";
                return;
            }

            try
            {
                IsLoading = true;
                LoadingMessage = $"Changing time for {SelectedJob.Name}...";
                ErrorMessage = "";

                var selectedJobName = SelectedJob.Name;
                var success = await _apiService.UpdateJobTimeAsync(selectedJobName, newTime.Trim());

                if (!success)
                {
                    ErrorMessage = "Failed to change time.";
                    return;
                }

                await LoadAsync();
                SelectedJob = Jobs.FirstOrDefault(j => j.Name == selectedJobName);

                SchedulerMessage = $"Changed time for {selectedJobName} to {newTime.Trim()}";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                LoadingMessage = "";
                IsLoading = false;
            }
        }

        public async Task ChangeSelectedJobDaysAsync(List<string> newDays)
        {
            if (SelectedJob == null)
                return;

            if (newDays == null || newDays.Count == 0)
            {
                ErrorMessage = "Select at least one day.";
                return;
            }

            try
            {
                IsLoading = true;
                LoadingMessage = $"Changing days for {SelectedJob.Name}...";
                ErrorMessage = "";

                var selectedJobName = SelectedJob.Name;

                var success = await _apiService.UpdateJobDaysAsync(selectedJobName, newDays);

                if (!success)
                {
                    ErrorMessage = "Failed to change days.";
                    return;
                }

                await LoadAsync();
                SelectedJob = Jobs.FirstOrDefault(j => j.Name == selectedJobName);

                SchedulerMessage = $"Changed days for {selectedJobName}";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                LoadingMessage = "";
                IsLoading = false;
            }
        }

        public async Task RenameSelectedJobAsync(string newName)
        {
            if (SelectedJob == null)
                return;

            if (string.IsNullOrWhiteSpace(newName))
            {
                ErrorMessage = "New job name cannot be empty.";
                return;
            }

            try
            {
                IsLoading = true;
                LoadingMessage = $"Renaming {SelectedJob.Name}...";
                ErrorMessage = "";

                var oldName = SelectedJob.Name;
                var cleanNewName = newName?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(cleanNewName))
                {
                    ErrorMessage = "New job name cannot be empty.";
                    return;
                }

                var invalidChars = new[] { '/', '\\', '?', '#', '%', '*', ':', '[', ']' };

                if (cleanNewName.Any(c => invalidChars.Contains(c)))
                {
                    ErrorMessage = "Invalid characters: / \\ ? # % * : [ ]";
                    return;
                }

                var success = await _apiService.RenameJobAsync(oldName, cleanNewName);

                if (!success)
                {
                    ErrorMessage = "Failed to rename job. The name may already exist.";
                    return;
                }

                await LoadAsync();
                SelectedJob = Jobs.FirstOrDefault(j => j.Name == cleanNewName);

                SchedulerMessage = $"Renamed job to {cleanNewName}";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                LoadingMessage = "";
                IsLoading = false;
            }
        }
    }
}