using Blaise.Case.Backup.MessageBroker.Models;

namespace Blaise.Case.Backup.MessageBroker.Interfaces
{
    public interface IMessageModelMapper
    {
        MessageModel MapToMessageModel(string message);
    }
}