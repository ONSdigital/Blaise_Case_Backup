using System;
using System.Configuration;
using BlaiseCaseBackup.Interfaces;

namespace BlaiseCaseBackup.Providers
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public string TimerIntervalInMinutes => ConfigurationManager.AppSettings["TimerIntervalInMinutes"];

        public string BucketName => ConfigurationManager.AppSettings["BucketName"];

        public string VmName => Environment.MachineName;
    }
}
