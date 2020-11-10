using System.Collections.Generic;
using System.Linq;
using Blaise.Case.Backup.Data.Interfaces;
using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Case.Backup.Data
{
    public class BlaiseApiService : IBlaiseApiService
    {
        private readonly IBlaiseApi _blaiseApi;

        public BlaiseApiService(IBlaiseApi blaiseApi)
        {
            _blaiseApi = blaiseApi;
        }

        public List<ISurvey> GetAvailableSurveys()
        {
            return _blaiseApi.GetAllSurveys(_blaiseApi.GetDefaultConnectionModel()).ToList();
        }

        public void BackupSurveyToFile(string serverPark, string instrumentName, string outputPath)
        {
            _blaiseApi.BackupSurveyToFile(_blaiseApi.GetDefaultConnectionModel(), serverPark, instrumentName, outputPath);
        }
    }
}
