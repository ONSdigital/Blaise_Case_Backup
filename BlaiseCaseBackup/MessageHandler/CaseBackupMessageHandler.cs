using System;
using Blaise.Nuget.PubSub.Contracts.Interfaces;
using BlaiseCaseBackup.Enums;
using BlaiseCaseBackup.Interfaces;
using log4net;

namespace BlaiseCaseBackup.MessageHandler
{
    public class CaseBackupMessageHandler : IMessageHandler
    {
        private readonly ILog _logger;
        private readonly IServiceActionMapper _mapper;
        private readonly IBackupSurveysService _backupSurveysService;

        public CaseBackupMessageHandler(
            ILog logger, 
            IServiceActionMapper mapper, 
            IBackupSurveysService backupSurveysService)
        {
            _logger = logger;
            _mapper = mapper;
            _backupSurveysService = backupSurveysService;
        }

        public bool HandleMessage(string message)
        {
            try
            {
                _logger.Info($"Message received '{message}'");

                var messageModel = _mapper.MapToCaseBackupActionModel(message);

                if (messageModel.Action != ActionType.StartBackup)
                {
                    _logger.Warn("The message received could not be processed");

                    return true;
                }

                _backupSurveysService.BackupSurveys();

                return true;
            }
            catch (Exception ex)
            {
                _logger.Info($"Error processing message '{message}', with exception {ex}");

                return false;
            }
        }
    }
}
