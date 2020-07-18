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
            foreach (var survey in _blaiseApi.Surveys)
            {
                _logger.Info($"Processing survey '{survey.Name}' for server park '{survey.ServerPark}'");

                _blaiseApi
                    .WithInstrument(survey.Name)
                    .WithServerPark(survey.ServerPark)
                    .Survey
                    .ToPath(_configurationProvider.BackupPath)
                    .Backup();

                _logger.Info($"Backed up survey '{survey.Name}' for server park '{survey.ServerPark}' to file '{_configurationProvider.BackupPath}'");
            }
        }
    }
}
