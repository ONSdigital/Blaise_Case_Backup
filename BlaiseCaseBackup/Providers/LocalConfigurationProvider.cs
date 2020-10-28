using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlaiseCaseBackup.Providers
{
    public class LocalConfigurationProvider
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
