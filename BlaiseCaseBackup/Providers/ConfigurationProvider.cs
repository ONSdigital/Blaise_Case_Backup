using System;
using System.Configuration;
using BlaiseCaseBackup.Extensions;
using BlaiseCaseBackup.Interfaces;
using StatNeth.Blaise.Runtime.DataContract.Rules.Interfaces;

namespace BlaiseCaseBackup.Providers
{
    public class ConfigurationProvider : IConfigurationProvider
    {
     
        public string BucketName => GetVariable("ENV_BCB_BUCKET_NAME");

        public string VmName => Environment.MachineName;

        public string ProjectId => GetVariable("ENV_PROJECT_ID");
        public string SubscriptionTopicId => GetVariable("ENV_BCB_SUB_TOPIC");
        public string SubscriptionId => GetVariable("ENV_BCB_SUB_SUBS");
        public string DeadletterTopicId => GetVariable("ENV_DEADLETTER_TOPIC");
        public string LocalBackupFolder => GetVariable("ENV_BCB_LOCAL_BACKUP_DIR");
        public string SettingsFolder => GetVariable("ENV_SETTINGS_DIRECTORY");

        private static string GetVariable(string variableName)
        {
            var value = Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Machine);

            value.ThrowExceptionIfNull(variableName);

            return value;
        }
    }
}
