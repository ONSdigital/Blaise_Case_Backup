using System.Configuration;
using BlaiseCaseBackup.Interfaces;

namespace BlaiseCaseBackup.Providers
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public string TimerIntervalInMinutes => ConfigurationManager.AppSettings["TimerIntervalInMinutes"];

        public string BackupPath => ConfigurationManager.AppSettings["BackupPath"];
    }
}
