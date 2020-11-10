using System.Collections.Generic;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Case.Backup.Data.Interfaces
{
    public interface IBlaiseApiService
    {
        List<ISurvey> GetAvailableSurveys();
        void BackupSurveyToFile(string serverPark, string instrumentName, string outputPath);
    }
}
