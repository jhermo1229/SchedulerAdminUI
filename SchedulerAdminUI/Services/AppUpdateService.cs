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
                    "https://github.com/SchedulerAdminUI",
                    null,
                    false));
        }

        public async Task<string> CheckAndUpdateAsync(Action<int>? progress = null)
        {
            if (!_updateManager.IsInstalled)
                return "Updates only work after the app is installed from a Velopack release.";

            var updateInfo = await _updateManager.CheckForUpdatesAsync();

            if (updateInfo == null)
                return "You are already using the latest version.";

            await _updateManager.DownloadUpdatesAsync(updateInfo, progress);

            _updateManager.ApplyUpdatesAndRestart(updateInfo.TargetFullRelease);

            return "Update downloaded. Restarting...";
        }
    }
}