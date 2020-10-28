using Blaise.Case.Backup.Providers;
using NUnit.Framework;

namespace Blaise.Case.Backup.Tests.Unit.Providers
{
    public class LocalConfigurationProviderTests
    {
        [Test]
        public void Given_I_Call_BucketName_I_Get_The_Correct_Value_Back()
        {
            //arrange
            var configurationProvider = new LocalConfigurationProvider();

            //act
            var result = configurationProvider.BucketName;

            //assert
            Assert.AreEqual("BucketNameTest", result);
        }

        [Test]
        public void Given_I_Call_ProjectId_I_Get_The_Correct_Value_Back()
        {
            //arrange
            var configurationProvider = new LocalConfigurationProvider();

            //act
            var result = configurationProvider.ProjectId;

            //assert
            Assert.AreEqual("ProjectIdTest", result);
        }

        [Test]
        public void Given_I_Call_SubscriptionTopicId_I_Get_The_Correct_Value_Back()
        {
            //arrange
            var configurationProvider = new LocalConfigurationProvider();

            //act
            var result = configurationProvider.SubscriptionTopicId;

            //assert
            Assert.AreEqual("SubscriptionTopicIdTest", result);
        }

        [Test]
        public void Given_I_Call_SubscriptionId_I_Get_The_Correct_Value_Back()
        {
            //arrange
            var configurationProvider = new LocalConfigurationProvider();

            //act
            var result = configurationProvider.SubscriptionId;

            //assert
            Assert.AreEqual("SubscriptionIdTest", result);
        }

        [Test]
        public void Given_I_Call_DeadletterTopicId_I_Get_The_Correct_Value_Back()
        {
            //arrange
            var configurationProvider = new LocalConfigurationProvider();

            //act
            var result = configurationProvider.DeadletterTopicId;

            //assert
            Assert.AreEqual("DeadletterTopicIdTest", result);
        }

        [Test]
        public void Given_I_Call_LocalBackupFolder_I_Get_The_Correct_Value_Back()
        {
            //arrange
            var configurationProvider = new LocalConfigurationProvider();

            //act
            var result = configurationProvider.LocalBackupFolder;

            //assert
            Assert.AreEqual("LocalBackupFolderTest", result);
        }

        [Test]
        public void Given_I_Call_SettingsFolder_I_Get_The_Correct_Value_Back()
        {
            //arrange
            var configurationProvider = new LocalConfigurationProvider();

            //act
            var result = configurationProvider.SettingsFolder;

            //assert
            Assert.AreEqual("SettingsFolderTest", result);
        }
    }
}
