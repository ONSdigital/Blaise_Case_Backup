using BlaiseCaseBackup.Providers;
using NUnit.Framework;

namespace BlaiseCaseBackup.Tests.Providers
{
    public class ConfigurationProviderTests
    {
        [Test]
        public void Given_I_Call_TimerIntervalInMinutes_I_Get_The_Correct_Value_Back()
        {
            //arrange
            var configurationProvider = new ConfigurationProvider();

            //act
            var result = configurationProvider.TimerIntervalInMinutes;

            //assert
            Assert.AreEqual("5", result);
        }

        [Test]
        public void Given_I_Call_BackupPath_I_Get_The_Correct_Value_Back()
        {
            //arrange
            var configurationProvider = new ConfigurationProvider();

            //act
            var result = configurationProvider.BackupPath;

            //assert
            Assert.AreEqual("BackupPathTest", result);
        }
    }
}
