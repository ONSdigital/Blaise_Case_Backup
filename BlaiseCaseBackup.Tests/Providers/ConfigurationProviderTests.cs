using BlaiseCaseBackup.Providers;
using NUnit.Framework;
using System.Configuration;

namespace BlaiseCaseBackup.Tests.Providers
{
    public class ConfigurationProviderTests
    {

        [Test]
        public void Given_I_Call_BucketName_And_The_Env_Variable_Is_Not_Set_Then_A_ConfigurationErrorsException_Is_Thrown()
        {
            //arrange
            var configurationProvider = new ConfigurationProvider();

            //act
            var exception = Assert.Throws<ConfigurationErrorsException>(() =>
            { 
            var result = configurationProvider.BucketName;

            });

            //assert
            Assert.AreEqual("No value found for environment variable 'ENV_BCB_BUCKET_NAME'", exception.Message);
        }

        [Test]
        public void Given_I_Call_ProjectId_And_The_Env_Variable_Is_Not_Set_Then_A_ConfigurationErrorsException_Is_Thrown()
        {
            //arrange
            var configurationProvider = new ConfigurationProvider();

            //act
            var exception = Assert.Throws<ConfigurationErrorsException>(() =>
            {
                var result = configurationProvider.ProjectId;
            });

            //assert
            Assert.AreEqual("No value found for environment variable 'ENV_PROJECT_ID'", exception.Message);
        }

        [Test]
        public void Given_I_Call_SubscriptionTopicId_And_The_Env_Variable_Is_Not_Set_Then_A_ConfigurationErrorsException_Is_Thrown()
        {
            //arrange
            var configurationProvider = new ConfigurationProvider();

            //act
            var exception = Assert.Throws<ConfigurationErrorsException>(() =>
            {
                var result = configurationProvider.SubscriptionTopicId;
            });

            //assert
            Assert.AreEqual("No value found for environment variable 'ENV_BCB_SUB_TOPIC'", exception.Message);
        }

        [Test]
        public void Given_I_Call_SubscriptionId_And_The_Env_Variable_Is_Not_Set_Then_A_ConfigurationErrorsException_Is_Thrown()
        {
            //arrange
            var configurationProvider = new ConfigurationProvider();

            //act
            var exception = Assert.Throws<ConfigurationErrorsException>(() =>
            {
                var result = configurationProvider.SubscriptionTopicId;
            });

            //assert
            Assert.AreEqual("No value found for environment variable 'ENV_BCB_SUB_TOPIC'", exception.Message);
        }

        [Test]
        public void Given_I_Call_DeadletterTopicId_And_The_Env_Variable_Is_Not_Set_Then_A_ConfigurationErrorsException_Is_Thrown()
        {
            //arrange
            var configurationProvider = new ConfigurationProvider();

            //act
            var exception = Assert.Throws<ConfigurationErrorsException>(() =>
            {
                var result = configurationProvider.DeadletterTopicId;
            });

            //assert
            Assert.AreEqual("No value found for environment variable 'ENV_DEADLETTER_TOPIC'", exception.Message);
        }

        [Test]
        public void Given_I_Call_LocalBackupFolder_And_The_Env_Variable_Is_Not_Set_Then_A_ConfigurationErrorsException_Is_Thrown()
        {
            //arrange
            var configurationProvider = new ConfigurationProvider();

            //act
            var exception = Assert.Throws<ConfigurationErrorsException>(() =>
            {
                var result = configurationProvider.LocalBackupFolder;
            });

            //assert
            Assert.AreEqual("No value found for environment variable 'ENV_BCB_LOCAL_BACKUP_DIR'", exception.Message);
        }

        [Test]
        public void Given_I_Call_SettingsFolder_And_The_Env_Variable_Is_Not_Set_Then_A_ConfigurationErrorsException_Is_Thrown()
        {
            //arrange
            var configurationProvider = new ConfigurationProvider();

            //act
            var exception = Assert.Throws<ConfigurationErrorsException>(() =>
            {
                var result = configurationProvider.SettingsFolder;
            });

            //assert
            Assert.AreEqual("No value found for environment variable 'ENV_SETTINGS_DIRECTORY'", exception.Message);
        }
    }
}
