using BlaiseCaseBackup.Models;

namespace BlaiseCaseBackup.Interfaces
{
    public interface IServiceActionMapper
    {
        CaseBackupActionModel MapToCaseBackupActionModel(string message);
    }
}