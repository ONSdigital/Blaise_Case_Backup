using BlaiseCaseBackup.Enums;
using BlaiseCaseBackup.Interfaces;
using BlaiseCaseBackup.Models;
using Newtonsoft.Json;

namespace BlaiseCaseBackup.Mappers
{
    public class ServiceActionMapper : IServiceActionMapper
    {
        public CaseBackupActionModel MapToCaseBackupActionModel(string message)
        {
            try
            {
                return JsonConvert.DeserializeObject<CaseBackupActionModel>(message);
            }
            catch
            {
                // This is horrible I know but we currently don't really care about the message as it is only a trigger
                // and we need to ensure a message incorrectly put on this topic does not trigger it
            }

            return new CaseBackupActionModel { Action = ActionType.NotSupported };
        }
    }
}
