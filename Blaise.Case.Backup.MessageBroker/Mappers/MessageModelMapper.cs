using Blaise.Case.Backup.MessageBroker.Enums;
using Blaise.Case.Backup.MessageBroker.Interfaces;
using Blaise.Case.Backup.MessageBroker.Models;
using Newtonsoft.Json;

namespace Blaise.Case.Backup.MessageBroker.Mappers
{
    public class MessageModelMapper : IMessageModelMapper
    {
        public MessageModel MapToMessageModel(string message)
        {
            try
            {
                return JsonConvert.DeserializeObject<MessageModel>(message);
            }
            catch
            {
                // This is horrible I know but we currently don't really care about the message as it is only a trigger
                // and we need to ensure a message incorrectly put on this topic does not trigger it
            }

            return new MessageModel { Action = ActionType.NotSupported };
        }
    }
}
