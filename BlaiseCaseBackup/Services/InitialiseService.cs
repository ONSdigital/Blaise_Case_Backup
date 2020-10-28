using System;
using Blaise.Nuget.PubSub.Contracts.Interfaces;
using BlaiseCaseBackup.Interfaces;
using log4net;

namespace BlaiseCaseBackup.Services
{
    public class InitialiseService : IInitialiseService
    {
        private readonly ILog _logger;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IQueueService _queueService;
        private readonly IMessageHandler _messageHandler;

        public InitialiseService(
            ILog logger,
            IQueueService queueService,
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
                throw;
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
