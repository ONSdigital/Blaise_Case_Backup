using BlaiseCaseBackup.Interfaces;
using BlaiseCaseBackup.Services;
using log4net;
using Moq;
using NUnit.Framework;

namespace BlaiseCaseBackup.Tests.Services
{
    public class InitialiseServiceTests
    {
        private Mock<ILog> _loggerMock;
        private Mock<IConfigurationProvider> _configurationProviderMock;
        private Mock<IBackupSurveysService> _backupSurveysService;

        private InitialiseService _sut;

        [SetUp]
        public void SetUpTests()
        {
            _loggerMock = new Mock<ILog>();
            _configurationProviderMock = new Mock<IConfigurationProvider>();
            _configurationProviderMock.Setup(c => c.TimerIntervalInMinutes).Returns("1");

            _backupSurveysService = new Mock<IBackupSurveysService>();

            _sut = new InitialiseService(
                _loggerMock.Object,
                _configurationProviderMock.Object,
                _backupSurveysService.Object);
        }

        [Test]
        public void Given_I_Call_Start_Then_The_Correct_Methods_Are_Called_To_Setup_The_Service()
        {
            //arrange

            //act
            _sut.Start();

            //assert
            _loggerMock.Verify(v => v.Info(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Given_I_Call_Stop_Then_The_Appropriate_Service_Is_Called()
        {
            //act
            _sut.Stop();

            //assert
            _loggerMock.Verify(v => v.Info(It.IsAny<string>()), Times.Once);
        }
    }
}
