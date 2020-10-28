using System;
using Blaise.Case.Backup.Enums;
using Blaise.Case.Backup.Interfaces;
using Blaise.Nuget.PubSub.Contracts.Interfaces;
using log4net;

namespace Blaise.Case.Backup.MessageHandler
{
    public class CaseBackupMessageHandler : IMessageHandler
    {
        private readonly ILog _logger;
        private readonly IServiceActionMapper _mapper;
        private readonly IBackupService _backupService;

        public CaseBackupMessageHandler(
            ILog logger, 
            IServiceActionMapper mapper, 
            IBackupService backupService)
        {
            _logger = logger;
            _mapper = mapper;
            _backupService = backupService;
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

                _backupService.BackupSurveys();
                _backupService.BackupSettings();

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
