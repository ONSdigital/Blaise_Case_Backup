using System;
using Blaise.Nuget.PubSub.Contracts.Interfaces;
using BlaiseCaseBackup.Interfaces;
using BlaiseCaseBackup.Services;
using log4net;
using Moq;
using NUnit.Framework;

namespace BlaiseCaseBackup.Tests.Services
{
    public class InitialiseServiceTests
    {
        private Mock<ILog> _loggingMock;
        private Mock<IQueueService> _queueServiceMock;
        private Mock<IMessageHandler> _messageHandlerMock;
        private Mock<IConfigurationProvider> _configurationProviderMock;

        private InitialiseService _sut;

        [SetUp]
        public void SetUpTests()
        {
            _loggingMock = new Mock<ILog>();
            _queueServiceMock = new Mock<IQueueService>();
            _messageHandlerMock = new Mock<IMessageHandler>();
            _configurationProviderMock = new Mock<IConfigurationProvider>();

            _sut = new InitialiseService(
                _loggingMock.Object,
                _queueServiceMock.Object,
                _messageHandlerMock.Object,
                _configurationProviderMock.Object);
        }

        [Test]
        public void Given_I_Call_Start_Then_The_Correct_Methods_Are_Called_To_Setup_And_Subscribe_To_The_Appropriate_Queues()
        {
            //act
            _sut.Start();

            //assert
            _queueServiceMock.Verify(v => v.Subscribe(It.IsAny<IMessageHandler>()), Times.Once);
        }

        [Test]
        public void Given_I_Call_Stop_Then_The_Appropriate_Service_Is_Called()
        {
            //act
            _sut.Stop();

            //assert
            _queueServiceMock.Verify(v => v.CancelAllSubscriptions(), Times.Once);
        }
    }
}
