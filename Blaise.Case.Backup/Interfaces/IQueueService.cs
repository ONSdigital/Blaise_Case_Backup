
using Blaise.Nuget.PubSub.Contracts.Interfaces;

namespace Blaise.Case.Backup.Interfaces
{
    public interface IQueueService
    {
        void Subscribe(IMessageHandler messageHandler);

        void CancelAllSubscriptions();
    }
}