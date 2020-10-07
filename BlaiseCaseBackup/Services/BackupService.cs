using System.Collections.Generic;
using System.Linq;
using Blaise.Nuget.Api.Contracts.Interfaces;
using BlaiseCaseBackup.Interfaces;
using log4net;
using StatNeth.Blaise.API.ServerManager;

namespace BlaiseCaseBackup.Services
{
    public class BackupService : IBackupService
    {
        private readonly ILog _logger;
        private readonly IFluentBlaiseApi _blaiseApi;
        private readonly IConfigurationProvider _configurationProvider;

        public BackupService(
            ILog logger, 
            IFluentBlaiseApi blaiseApi, 
            IConfigurationProvider configurationProvider)
        {
            _logger = logger;
            _blaiseApi = blaiseApi;
            _configurationProvider = configurationProvider;
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
                var bucketFolderPath = $"{survey.ServerPark}";

                BackupSurvey(survey, localFolderPath, bucketFolderPath);

                _logger.Info($"Backed up survey '{survey.Name}' for server park '{survey.ServerPark}' to bucket '{_configurationProvider.BucketName}' on '{_configurationProvider.VmName}'");
            }
        }

        public void BackupSettings()
        {
            var bucketPath = $"{_configurationProvider.VmName}/Settings";

            _blaiseApi
                .Settings
                .WithSourceFolder(_configurationProvider.SettingsFolder)
                .ToBucket(_configurationProvider.BucketName, bucketPath)
                .Backup();
        }

        private List<ISurvey> GetAvailableSurveys()
        {
            return _blaiseApi
                .WithConnection(_blaiseApi.DefaultConnection)
                .Surveys.ToList();
        }

        private void BackupSurvey(ISurvey survey, string localFolderPath, string bucketFolderPath)
        {
            _blaiseApi
                .WithConnection(_blaiseApi.DefaultConnection)
                .WithInstrument(survey.Name)
                .WithServerPark(survey.ServerPark)
                .Survey
                .ToPath(localFolderPath)
                .ToBucket(_configurationProvider.BucketName, bucketFolderPath)
                .Backup();
        }
    }
}
