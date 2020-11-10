using System;
using Blaise.Case.Backup.Core.Interfaces;
using Blaise.Case.Backup.MessageBroker.Enums;
using Blaise.Case.Backup.MessageBroker.Interfaces;
using Blaise.Nuget.PubSub.Contracts.Interfaces;
using log4net;

namespace Blaise.Case.Backup.MessageBroker
{
    public class MessageHandler : IMessageHandler
    {
        private readonly ILog _logger;
        private readonly IMessageModelMapper _mapper;
        private readonly IBackupService _backupService;

        public MessageHandler(
            ILog logger, 
            IMessageModelMapper mapper, 
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

                var messageModel = _mapper.MapToMessageModel(message);

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
