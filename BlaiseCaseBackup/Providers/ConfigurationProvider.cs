using System;
using System.Configuration;
using BlaiseCaseBackup.Extensions;
using BlaiseCaseBackup.Interfaces;

namespace BlaiseCaseBackup.Providers
{
    public class ConfigurationProvider : IConfigurationProvider
    {
     
        public string BucketName => Environment.GetEnvironmentVariable("ENV_BCB_BUCKET_NAME", EnvironmentVariableTarget.Machine)
                                    .ThrowExceptionIfNotNull();

        public string VmName => Environment.MachineName;

        public string ProjectId => Environment.GetEnvironmentVariable("ENV_PROJECT_ID", EnvironmentVariableTarget.Machine)
                                    .ThrowExceptionIfNotNull();
        public string SubscriptionTopicId => Environment.GetEnvironmentVariable("ENV_BCB_SUB_TOPIC", EnvironmentVariableTarget.Machine)
                                    .ThrowExceptionIfNotNull();
        public string SubscriptionId => Environment.GetEnvironmentVariable("ENV_BCB_SUB_SUBS", EnvironmentVariableTarget.Machine)
                                    .ThrowExceptionIfNotNull();
        public string DeadletterTopicId => Environment.GetEnvironmentVariable("ENV_DEADLETTER_TOPIC", EnvironmentVariableTarget.Machine)
                                    .ThrowExceptionIfNotNull();
        public string LocalBackupFolder => Environment.GetEnvironmentVariable("ENV_BCB_LOCAL_BACKUP_DIR", EnvironmentVariableTarget.Machine)
                                    .ThrowExceptionIfNotNull();
        public string SettingsFolder => Environment.GetEnvironmentVariable("ENV_SETTINGS_DIRECTORY", EnvironmentVariableTarget.Machine)
                                    .ThrowExceptionIfNotNull();
    }
}
