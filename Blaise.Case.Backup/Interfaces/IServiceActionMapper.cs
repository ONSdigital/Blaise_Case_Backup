using Blaise.Case.Backup.Models;

namespace Blaise.Case.Backup.Interfaces
{
    public interface IServiceActionMapper
    {
        CaseBackupActionModel MapToCaseBackupActionModel(string message);
    }
}