using System.Globalization;
using System.Timers;
using BlaiseCaseBackup.Interfaces;
using log4net;

namespace BlaiseCaseBackup.Services
{
    public class InitialiseService : IInitialiseService
    {
        private readonly ILog _logger;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IBackupSurveysService _backupSurveysService;

        public InitialiseService(
            ILog logger,
            IConfigurationProvider configurationProvider,
            IBackupSurveysService backupSurveysService)
        {
            _logger = logger;
            _configurationProvider = configurationProvider;
            _backupSurveysService = backupSurveysService;
        }

        public void Start()
        {
            var timerIntervalInMinutes = _configurationProvider.TimerIntervalInMinutes;
            var time = double.Parse(timerIntervalInMinutes, CultureInfo.InvariantCulture.NumberFormat);
            time = time * 60 * 1000;

            // Set up a timer that triggers every minute.
            var timer = new Timer { Interval = time };
            timer.Elapsed += BackupSurveys;
            timer.Start();

            _logger.Info($"Blaise Case Backup service started on '{_configurationProvider.VmName}'");
        }

        public void Stop()
        {
            _logger.Info($"Blaise Case Backup service stopped  on '{_configurationProvider.VmName}'");
        }

        private void BackupSurveys(object sender, ElapsedEventArgs args)
        {
            _backupSurveysService.BackupSurveys();
        }
    }
}
