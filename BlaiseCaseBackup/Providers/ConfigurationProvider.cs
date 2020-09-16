using System;
using System.Configuration;
using BlaiseCaseBackup.Interfaces;

namespace BlaiseCaseBackup.Providers
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public string BucketName => ConfigurationManager.AppSettings["BucketName"];

        public string VmName => Environment.MachineName;

        public string ProjectId => Environment.GetEnvironmentVariable("ENV_PROJECT_ID", EnvironmentVariableTarget.Machine)
                                   ?? ConfigurationManager.AppSettings["ProjectId"];

        public string SubscriptionTopicId => ConfigurationManager.AppSettings["SubscriptionTopicId"];

        public string SubscriptionId => ConfigurationManager.AppSettings["SubscriptionId"];

        public string DeadletterTopicId => ConfigurationManager.AppSettings["DeadletterTopicId"];

        public string LocalBackupFolder => ConfigurationManager.AppSettings["LocalBackupFolder"];
    }
}
