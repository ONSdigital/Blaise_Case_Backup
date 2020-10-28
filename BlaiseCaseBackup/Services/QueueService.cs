using System;
using Blaise.Nuget.PubSub.Contracts.Interfaces;
using BlaiseCaseBackup.Interfaces;
using log4net;

namespace BlaiseCaseBackup.Services
{
    public class QueueService : IQueueService
    {
        private readonly ILog _logger;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IFluentQueueApi _queueApi;

        public QueueService(
            ILog logger,
            IConfigurationProvider configurationProvider,
            IFluentQueueApi queueApi)
        {
            _logger = logger;
            _configurationProvider = configurationProvider;
            _queueApi = queueApi;
        }

        public void Subscribe(IMessageHandler messageHandler)
        {
            _queueApi
                .WithProject(_configurationProvider.ProjectId)
                .WithSubscription(_configurationProvider.SubscriptionId)
                .WithExponentialBackOff(60)
                .WithDeadLetter(_configurationProvider.DeadletterTopicId)
                .StartConsuming(messageHandler, true);

            _logger.Info($"Subscription setup to '{_configurationProvider.SubscriptionId}' " +
                         $"for project '{_configurationProvider.ProjectId}'");
        }

        public void CancelAllSubscriptions()
        {
            try
            {
                _queueApi
                    .StopConsuming();

            }
            catch (Exception e)
            {
                _logger.Error($"Could not stop consuming subscription because '{e.Message}'");
                throw;
            }

            _logger.Info($"Stopped consuming Subscription to '{_configurationProvider.SubscriptionId}' " +
                         $"for project '{_configurationProvider.ProjectId}'");
        }
    }
}
