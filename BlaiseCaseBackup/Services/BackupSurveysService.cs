using Blaise.Nuget.Api.Contracts.Interfaces;
using BlaiseCaseBackup.Interfaces;
using log4net;

namespace BlaiseCaseBackup.Services
{
    public class BackupSurveysService : IBackupSurveysService
    {
        private readonly ILog _logger;
        private readonly IFluentBlaiseApi _blaiseApi;
        private readonly IConfigurationProvider _configurationProvider;

        public BackupSurveysService(
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
            foreach (var survey in 
                _blaiseApi
                    .WithConnection(_blaiseApi.DefaultConnection)
                    .Surveys)
            {
                _logger.Info($"Processing survey '{survey.Name}' for server park '{survey.ServerPark}' on '{_configurationProvider.VmName}'");

                var localFolderPath = $"{_configurationProvider.LocalBackupFolder}/{survey.ServerPark}";
                var folderPath = $"{survey.ServerPark}";

                _blaiseApi
                    .WithConnection(_blaiseApi.DefaultConnection)
                    .WithInstrument(survey.Name)
                    .WithServerPark(survey.ServerPark)
                    .Survey
                    .ToPath(localFolderPath)
                    .ToBucket(_configurationProvider.BucketName, folderPath)
                    .Backup();

                _logger.Info($"Backed up survey '{survey.Name}' for server park '{survey.ServerPark}' to bucket '{_configurationProvider.BucketName}' on '{_configurationProvider.VmName}'");
            }
        }
    }
}
