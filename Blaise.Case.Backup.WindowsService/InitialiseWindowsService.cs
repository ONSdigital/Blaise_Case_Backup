using System;
using Blaise.Case.Backup.Core.Interfaces;
using Blaise.Case.Backup.MessageBroker.Interfaces;
using Blaise.Case.Backup.WindowsService.Interfaces;
using Blaise.Nuget.PubSub.Contracts.Interfaces;
using log4net;

namespace Blaise.Case.Backup.WindowsService
{
    public class InitialiseWindowsService : IInitialiseWindowsService
    {
        private readonly ILog _logger;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IMessageBrokerService _queueService;
        private readonly IMessageHandler _messageHandler;

        public InitialiseWindowsService(
            ILog logger,
            IMessageBrokerService queueService,
            IMessageHandler messageHandler,
            IConfigurationProvider configurationProvider)
        {
            _logger = logger;
            _queueService = queueService;
            _messageHandler = messageHandler;
            _configurationProvider = configurationProvider;
        }

        public void Start()
        {
            _logger.Info($"Starting case backup service on '{_configurationProvider.VmName}'");

            try
            {
                _queueService.Subscribe(_messageHandler);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            _logger.Info($"Starting case backup service started on '{_configurationProvider.VmName}'");
        }

        public void Stop()
        {
            _logger.Info($"Stopping case backup service on '{_configurationProvider.VmName}'");

            _queueService.CancelAllSubscriptions();

            _logger.Info($"Starting case backup service stopped on '{_configurationProvider.VmName}'");
        }
    }
}
