using System.Configuration;
using Blaise.Case.Backup.Core.Interfaces;

namespace Blaise.Case.Backup.Core.Configuration
{
    public class LocalConfigurationProvider : IConfigurationProvider
    {
        public string BucketName => ConfigurationManager.AppSettings["BucketName"];

        public string VmName => ConfigurationManager.AppSettings["VmName"];

        public string ProjectId => ConfigurationManager.AppSettings["ProjectId"];

        public string SubscriptionId => ConfigurationManager.AppSettings["SubscriptionId"];

        public string DeadletterTopicId => ConfigurationManager.AppSettings["DeadletterTopicId"];

        public string LocalBackupFolder => ConfigurationManager.AppSettings["LocalBackupFolder"];
        public string SettingsFolder => ConfigurationManager.AppSettings["SettingsFolder"];
    }
}
