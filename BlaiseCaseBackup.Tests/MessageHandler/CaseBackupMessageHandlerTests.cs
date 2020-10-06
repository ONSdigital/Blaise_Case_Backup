using System;
using BlaiseCaseBackup.Enums;
using BlaiseCaseBackup.Interfaces;
using BlaiseCaseBackup.MessageHandler;
using BlaiseCaseBackup.Models;
using log4net;
using Moq;
using NUnit.Framework;

namespace BlaiseCaseBackup.Tests.MessageHandler
{
    public class CaseBackupMessageHandlerTests
    {
        private Mock<ILog> _loggingMock;
        private Mock<IBackupSurveysService> _backupSurveysServiceMock;
        private Mock<IServiceActionMapper> _mapperMock;

        private readonly string _message;
        private readonly CaseBackupActionModel _actionModel;

        private CaseBackupMessageHandler _sut;

        public CaseBackupMessageHandlerTests()
        {
            _message = "Message";
            _actionModel = new CaseBackupActionModel { Action = ActionType.StartBackup};
        }

        [SetUp]
        public void SetUpTests()
        {
            _loggingMock = new Mock<ILog>();

            _backupSurveysServiceMock = new Mock<IBackupSurveysService>();

            _mapperMock = new Mock<IServiceActionMapper>();
            _mapperMock.Setup(m => m.MapToCaseBackupActionModel(_message)).Returns(_actionModel);

            _sut = new CaseBackupMessageHandler(
                _loggingMock.Object,
                _mapperMock.Object,
                _backupSurveysServiceMock.Object);
        }

        [Test]
        public void Given_Process_Action_Is_Not_Set_When_I_Call_HandleMessage_Then_True_Is_Returned()
        {
            //arrange
            _actionModel.Action = ActionType.NotSupported;

            //act
            var result = _sut.HandleMessage(_message);

            //assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result);
        }

        [Test]
        public void Given_Process_Action_Is_Not_Set_When_I_Call_HandleMessage_Then_Nothing_Is_Processed()
        {
            //arrange
            _actionModel.Action = ActionType.NotSupported;

            //act
            _sut.HandleMessage(_message);

            //assert
            _backupSurveysServiceMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Given_Process_Action_Is_Not_Set_When_I_Call_HandleMessage_Then_We_Log_Indicating_We_Could_Not_Process()
        {
            //arrange
            _actionModel.Action = ActionType.NotSupported;

            //act
            _sut.HandleMessage(_message);

            //assert
            _loggingMock.Verify(v => v.Warn("The message received could not be processed"), Times.Once);
        }

        [Test]
        public void Given_A_Inspect_Action_Is_Set_When_I_Call_HandleMessage_Then_True_Is_Returned()
        {
            //arrange
            _actionModel.Action = ActionType.StartBackup;

            //act
            var result = _sut.HandleMessage(_message);

            //assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result);
        }

        [Test]
        public void Given_A_Inspect_Action_Is_Set_When_I_Call_HandleMessage_Then_The_Correct_Services_Are_Called()
        {
            //arrange
            _actionModel.Action = ActionType.StartBackup;

            //act
            _sut.HandleMessage(_message);

            //assert
            _backupSurveysServiceMock.Verify(v => v.BackupSurveys(), Times.Once);
        }
        
        [Test]
        public void Given_An_Error_Occurs_When_I_Call_HandleMessage_Then_False_Is_Returned()
        {
            //arrange
            _backupSurveysServiceMock.Setup(c => c.BackupSurveys()).Throws(new Exception());

            //act 
            var result = _sut.HandleMessage(_message);

            //assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result);
        }

        [Test]
        public void Given_An_Error_Occurs_When_I_Call_HandleMessage_Then_Exception_Is_Handled_Correctly()
        {
            //arrange
            _backupSurveysServiceMock.Setup(c => c.BackupSurveys()).Throws(new Exception());

            //act && assert
            Assert.DoesNotThrow(() => _sut.HandleMessage(_message));
        }

        [Test]
        public void Given_An_Error_Occurs_When_I_Call_HandleMessage_Then_Nothing_Is_Processed()
        {
            //arrange
            _backupSurveysServiceMock.Setup(c => c.BackupSurveys()).Throws(new Exception());

            //act && assert
            _sut.HandleMessage(_message);
            _backupSurveysServiceMock.Verify(v =>v.BackupSurveys(), Times.Once);
            _backupSurveysServiceMock.VerifyNoOtherCalls();
        }
    }
}
