using System;
using System.Collections.Generic;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using BlaiseCaseBackup.Interfaces;
using BlaiseCaseBackup.Services;
using log4net;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.ServerManager;

namespace BlaiseCaseBackup.Tests.Services
{
    public class BackupSurveysServiceTests
    {
        private Mock<ILog> _loggingMock;
        private Mock<IFluentBlaiseApi> _blaiseApiMock;
        private Mock<IConfigurationProvider> _configurationProviderMock;

        private readonly string _instrumentName;
        private readonly string _serverPark;
        private readonly string _bucketName;
        private readonly string _localBackupPath;

        public BackupSurveysServiceTests()
        {
            _instrumentName = "Instrument1";
            _serverPark = "Park1";
            _bucketName = "OpnBucket";
            _localBackupPath = "BackupPath";
        }

        private BackupSurveysService _sut;

        [SetUp]
        public void SetUpTests()
        {
            _loggingMock = new Mock<ILog>();
            _blaiseApiMock = new Mock<IFluentBlaiseApi>();
            _blaiseApiMock.Setup(b => b.WithConnection(It.IsAny<ConnectionModel>())).Returns(_blaiseApiMock.Object);
            _blaiseApiMock.Setup(b => b.WithInstrument(It.IsAny<string>())).Returns(_blaiseApiMock.Object);
            _blaiseApiMock.Setup(b => b.WithServerPark(It.IsAny<string>())).Returns(_blaiseApiMock.Object);

            _blaiseApiMock.Setup(b => b.Survey.ToPath(It.IsAny<string>()).ToBucket(It.IsAny<string>(),
                It.IsAny<string>()).Backup());

            _configurationProviderMock = new Mock<IConfigurationProvider>();
            _configurationProviderMock.Setup(c => c.BucketName).Returns(_bucketName);
            _configurationProviderMock.Setup(c => c.LocalBackupFolder).Returns(_localBackupPath);


            _sut = new BackupSurveysService(
                _loggingMock.Object,
                _blaiseApiMock.Object,
                _configurationProviderMock.Object);
        }

        [Test]
        public void Given_I_Call_BackupSurveys_And_There_Are_No_Surveys_Then_No_Records_Are_Processed()
        {
            //arrange
            _blaiseApiMock.Setup(b => b.Surveys).Returns(new List<ISurvey>());

            //act
            _sut.BackupSurveys();

            //assert
            _blaiseApiMock.Verify(v => v.Surveys, Times.Once);
            _blaiseApiMock.Verify(v => v.Survey.Backup(), Times.Never);
        }

        [Test]
        public void Given_I_Call_BackupSurveys_And_There_Are_Surveys_Then_The_Surveys_Are_Backed_Up()
        {
            //arrange

            var surveyMock = new Mock<ISurvey>();
            surveyMock.Setup(s => s.Name).Returns(_instrumentName);
            surveyMock.Setup(s => s.ServerPark).Returns(_serverPark);

            _blaiseApiMock.Setup(b => b.Surveys).Returns(new List<ISurvey> { surveyMock.Object });

            var localFolderPath = $"{_localBackupPath}/{_serverPark}";
            var folderPath = $"{DateTime.Now.Date:yyyy-M-d}/{_serverPark}";

            //act
            _sut.BackupSurveys();

            //assert
            _blaiseApiMock.Verify(v => v.Surveys, Times.Once);
            _blaiseApiMock.Verify(v => v.WithInstrument(_instrumentName), Times.Once);
            _blaiseApiMock.Verify(v => v.WithServerPark(_serverPark), Times.Once);
            _blaiseApiMock.Verify(v => v.Survey
                .ToPath(localFolderPath).ToBucket(_bucketName, folderPath).Backup(), Times.Once);
        }
    }
}
