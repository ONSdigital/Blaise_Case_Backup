using System.Collections.Generic;
using System.Linq;
using Blaise.Case.Backup.CloudStorage.Interfaces;
using Blaise.Case.Backup.Core.Interfaces;
using Blaise.Case.Backup.Data.Interfaces;
using log4net;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Case.Backup.Core
{
    public class BackupService : IBackupService
    {
        private readonly ILog _logger;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IBlaiseApiService _blaiseApiService;
        private readonly IStorageService _bucketService;

        public BackupService(
            ILog logger,
            IConfigurationProvider configurationProvider,
            IBlaiseApiService blaiseApiService,
            IStorageService bucketService)
        {
            _logger = logger;
            _blaiseApiService = blaiseApiService;
            _configurationProvider = configurationProvider;
            _bucketService = bucketService;
        }

        public void BackupSurveys()
        {
            var surveys = GetAvailableSurveys();

            if (!surveys.Any())
            {
                _logger.Warn("There are no surveys available");

                return;
            }

            foreach (var survey in surveys)
            {
                _logger.Info($"Processing survey '{survey.Name}' for server park '{survey.ServerPark}' on '{_configurationProvider.VmName}'");

                var localFolderPath = $"{_configurationProvider.LocalBackupFolder}/{survey.ServerPark}";
                var bucketFolderPath = $"{_configurationProvider.VmName}/{survey.ServerPark}";

                BackupSurvey(survey, localFolderPath, bucketFolderPath);

                _logger.Info($"Backed up survey '{survey.Name}' for server park '{survey.ServerPark}' to bucket '{_configurationProvider.BucketName}' for '{_configurationProvider.VmName}'");
            }
        }

        public void BackupSettings()
        {
            _logger.Info($"Processing blaise setting files at '{_configurationProvider.SettingsFolder}' for '{_configurationProvider.VmName}'");

            var bucketFolderPath = $"{_configurationProvider.VmName}/Settings";

            _bucketService.BackupFilesToBucket(_configurationProvider.SettingsFolder, _configurationProvider.BucketName, bucketFolderPath);

            _logger.Info($"Blaise settings files backup up to bucket '{_configurationProvider.BucketName}' for '{_configurationProvider.VmName}'");
        }

        private List<ISurvey> GetAvailableSurveys()
        {
            return _blaiseApiService.GetAvailableSurveys().ToList();
        }

        private void BackupSurvey(ISurvey survey, string localFolderPath, string bucketFolderPath)
        {
            _blaiseApiService.BackupSurveyToFile(survey.ServerPark, survey.Name, localFolderPath);
            _bucketService.BackupFilesToBucket(localFolderPath, _configurationProvider.BucketName, bucketFolderPath);
        }
    }
}
