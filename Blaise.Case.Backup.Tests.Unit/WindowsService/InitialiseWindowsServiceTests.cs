using System;
using Blaise.Case.Backup.Core.Interfaces;
using Blaise.Case.Backup.MessageBroker.Interfaces;
using Blaise.Case.Backup.WindowsService;
using Blaise.Nuget.PubSub.Contracts.Interfaces;
using log4net;
using Moq;
using NUnit.Framework;

namespace Blaise.Case.Backup.Tests.Unit.WindowsService
{
    public class InitialiseWindowsServiceTests
    {
        private Mock<ILog> _loggingMock;
        private Mock<IMessageBrokerService> _messageBrokerMock;
        private Mock<IMessageHandler> _messageHandlerMock;
        private Mock<IConfigurationProvider> _configurationProviderMock;

        private InitialiseWindowsService _sut;

        [SetUp]
        public void SetUpTests()
        {
            _loggingMock = new Mock<ILog>();
            _messageBrokerMock = new Mock<IMessageBrokerService>();
            _messageHandlerMock = new Mock<IMessageHandler>();
            _configurationProviderMock = new Mock<IConfigurationProvider>();

            _sut = new InitialiseWindowsService(
                _loggingMock.Object,
                _messageBrokerMock.Object,
                _messageHandlerMock.Object,
                _configurationProviderMock.Object);
        }

        [Test]
        public void Given_I_Call_Start_Then_The_Correct_Methods_Are_Called_To_Setup_And_Subscribe_To_The_Appropriate_Queues()
        {
            //act
            _sut.Start();

            //assert
            _messageBrokerMock.Verify(v => v.Subscribe(It.IsAny<IMessageHandler>()), Times.Once);
        }

        [Test]
        public void Given_I_Call_Start_And_An_Exception_Is_Thrown_During_The_Process_Then_The_Exception_Is_Handled()
        {
            //arrange
            var exceptionThrown = new Exception("Error message");
            _messageBrokerMock.Setup(s => s.Subscribe(It.IsAny<IMessageHandler>())).Throws(exceptionThrown);
            _loggingMock.Setup(l => l.Error(It.IsAny<Exception>()));

            //act && assert
            Assert.DoesNotThrow(() => _sut.Start());
        }

        [Test]
        public void Given_I_Call_Start_And_An_Exception_Is_Thrown_During_The_Process_Then_The_Exception_Is_Logged()
        {
            //arrange
            var exceptionThrown = new Exception("Error message");
            _messageBrokerMock.Setup(s => s.Subscribe(It.IsAny<IMessageHandler>())).Throws(exceptionThrown);
            _loggingMock.Setup(l => l.Error(It.IsAny<Exception>()));

            //act
            _sut.Start();

            //assert
            _loggingMock.Verify(v => v.Error(exceptionThrown), Times.Once);
        }

        [Test]
        public void Given_I_Call_Stop_Then_The_Appropriate_Service_Is_Called()
        {
            //act
            _sut.Stop();

            //assert
            _messageBrokerMock.Verify(v => v.CancelAllSubscriptions(), Times.Once);
        }
    }
}
