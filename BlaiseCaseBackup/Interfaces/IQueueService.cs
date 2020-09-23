
using Blaise.Nuget.PubSub.Contracts.Interfaces;

namespace BlaiseCaseBackup.Interfaces
{
    public interface IQueueService
    {
        void Subscribe(IMessageHandler messageHandler);

        void CancelAllSubscriptions();
    }
}