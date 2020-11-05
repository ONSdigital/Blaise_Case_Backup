using System.Collections.Generic;
using System.Linq;
using Blaise.Case.Backup.Interfaces;
using Blaise.Nuget.Api.Contracts.Interfaces;
using log4net;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Case.Backup.Services
{
    public class BackupService : IBackupService
    {
        private readonly ILog _logger;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IBlaiseApi _blaiseApi;
        private readonly IBucketService _bucketService;

        public BackupService(
            ILog logger,
            IConfigurationProvider configurationProvider,
            IBlaiseApi blaiseApi,
            IBucketService bucketService)
        {
            _logger = logger;
            _blaiseApi = blaiseApi;
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
            return _blaiseApi.GetAllSurveys(_blaiseApi.GetDefaultConnectionModel()).ToList();
        }

        private void BackupSurvey(ISurvey survey, string localFolderPath, string bucketFolderPath)
        {
            _blaiseApi.BackupSurveyToFile(_blaiseApi.GetDefaultConnectionModel(), survey.ServerPark, survey.Name, localFolderPath);
            _bucketService.BackupFilesToBucket(localFolderPath, _configurationProvider.BucketName, bucketFolderPath);
        }
    }
}
