using Velopack;
using Velopack.Sources;

namespace SchedulerAdminUI.Services
{
    public class AppUpdateService
    {
        private readonly UpdateManager _updateManager;

        public AppUpdateService()
        {
            _updateManager = new UpdateManager(
                new GithubSource(
                    "https://github.com/jhermo1229/SchedulerAdminUI",
                    null,
                    false));
        }

        public async Task<string> CheckAndUpdateAsync(Action<int>? progress = null)
        {
            if (!_updateManager.IsInstalled)
                return "Updater only works after installing from Setup.exe.";

            try
            {
                var updateInfo = await _updateManager.CheckForUpdatesAsync();

                if (updateInfo == null)
                    return "You are already using the latest version.";

                await _updateManager.DownloadUpdatesAsync(updateInfo, progress);

                _updateManager.ApplyUpdatesAndRestart(updateInfo.TargetFullRelease);

                return "Update downloaded. Restarting...";
            }
            catch (Exception ex)
            {
                return $"Update check failed: {ex.Message}";
            }
        }
    }
}