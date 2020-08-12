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
            Assert.AreEqual("90", result);
        }

        [Test]
        public void Given_I_Call_BucketName_I_Get_The_Correct_Value_Back()
        {
            //arrange
            var configurationProvider = new ConfigurationProvider();

            //act
            var result = configurationProvider.BucketName;

            //assert
            Assert.AreEqual("BucketNameTest", result);
        }
    }
}
