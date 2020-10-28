using System.Configuration;
using BlaiseCaseBackup.Interfaces;

namespace BlaiseCaseBackup.Providers
{
    public class LocalConfigurationProvider : IConfigurationProvider
    {
        public string BucketName => ConfigurationManager.AppSettings["BucketName"];

        public string VmName => ConfigurationManager.AppSettings["VmName"];

        public string ProjectId => ConfigurationManager.AppSettings["ProjectId"];

        public string SubscriptionTopicId => ConfigurationManager.AppSettings["SubscriptionTopicId"];

        public string SubscriptionId => ConfigurationManager.AppSettings["SubscriptionId"];

        public string DeadletterTopicId => ConfigurationManager.AppSettings["DeadletterTopicId"];

        public string LocalBackupFolder => ConfigurationManager.AppSettings["LocalBackupFolder"];
        public string SettingsFolder => ConfigurationManager.AppSettings["SettingsFolder"];
    }
}
