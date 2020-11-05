using System.Collections.Generic;
using Blaise.Case.Backup.Interfaces;
using Blaise.Case.Backup.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using log4net;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Case.Backup.Tests.Unit.Services
{
    public class BackupServiceTests
    {
        private Mock<ILog> _loggingMock;
        private Mock<IConfigurationProvider> _configurationProviderMock;
        private Mock<IBlaiseApi> _blaiseApiMock;
        private Mock<IBucketService> _bucketServiceMock;

        private readonly ConnectionModel _connectionModel;

        private readonly string _instrumentName;
        private readonly string _serverParkName;
        private readonly string _bucketName;
        private readonly string _localBackupPath;
        private readonly string _vmName;

        public BackupServiceTests()
        {
            _connectionModel = new ConnectionModel();

            _instrumentName = "Instrument1";
            _serverParkName = "Park1";
            _bucketName = "OpnBucket";
            _localBackupPath = "BackupPath";
            _vmName = "Tel";
        }

        private BackupService _sut;

        [SetUp]
        public void SetUpTests()
        {
            _loggingMock = new Mock<ILog>();

            _configurationProviderMock = new Mock<IConfigurationProvider>();
            _configurationProviderMock.Setup(c => c.BucketName).Returns(_bucketName);
            _configurationProviderMock.Setup(c => c.LocalBackupFolder).Returns(_localBackupPath);
            _configurationProviderMock.Setup(c => c.VmName).Returns(_vmName);

            _blaiseApiMock = new Mock<IBlaiseApi>();
            _blaiseApiMock.Setup(b => b.GetDefaultConnectionModel()).Returns(_connectionModel);

            _bucketServiceMock = new Mock<IBucketService>();

            _sut = new BackupService(
                _loggingMock.Object,
                _configurationProviderMock.Object,
                _blaiseApiMock.Object,
                _bucketServiceMock.Object);              
        }

        [Test]
        public void Given_I_Call_BackupSurveys_And_There_Are_No_Surveys_Then_No_Records_Are_Processed()
        {
            //arrange
            _blaiseApiMock.Setup(b => b.GetAllSurveys(It.IsAny<ConnectionModel>())).Returns(new List<ISurvey>());

            //act
            _sut.BackupSurveys();

            //assert
            _blaiseApiMock.Verify(v => v.GetAllSurveys(_connectionModel), Times.Once);

            _blaiseApiMock.Verify(v => v.BackupSurveyToFile(It.IsAny<ConnectionModel>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            _bucketServiceMock.Verify(v => v.BackupFilesToBucket(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void Given_I_Call_BackupSurveys_And_There_Are_No_Surveys_Then_I_Log_A_Warning()
        {
            //arrange
            _blaiseApiMock.Setup(b => b.GetAllSurveys(It.IsAny<ConnectionModel>())).Returns(new List<ISurvey>());

            //act
            _sut.BackupSurveys();

            //assert
            _loggingMock.Verify(v => v.Warn("There are no surveys available"), Times.Once);
        }

        [Test]
        public void Given_I_Call_BackupSurveys_And_There_Are_Surveys_Then_The_Surveys_Are_Backed_Up()
        {
            //arrange

            var surveyMock = new Mock<ISurvey>();
            surveyMock.Setup(s => s.Name).Returns(_instrumentName);
            surveyMock.Setup(s => s.ServerPark).Returns(_serverParkName);

            _blaiseApiMock.Setup(b => b.GetAllSurveys(It.IsAny<ConnectionModel>())).Returns(new List<ISurvey> { surveyMock.Object });

            var localFolderPath = $"{_localBackupPath}/{_serverParkName}";
            var folderPath = $"{_vmName}/{_serverParkName}";

            //act
            _sut.BackupSurveys();

            //assert
            _blaiseApiMock.Verify(v => v.GetAllSurveys(_connectionModel), Times.Once);

            _blaiseApiMock.Verify(v => v.BackupSurveyToFile(_connectionModel, _serverParkName,
                _instrumentName, localFolderPath), Times.Once);

            _bucketServiceMock.Verify(v => v.BackupFilesToBucket(localFolderPath,
                _bucketName,  folderPath), Times.Once);
        }

        [Test]
        public void Given_I_Call_BackupSettings_And_There_Are_Settings_Files_Then_The_Files_Are_Backed_Up()
        {
            //arrange
            var settingsFolder = "SettingsFolder";
            var folderPath = $"{_vmName}/Settings";

            _configurationProviderMock.Setup(c => c.SettingsFolder).Returns(settingsFolder);

            _blaiseApiMock.Setup(b => b.BackupFilesToBucket(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            //act
            _sut.BackupSettings();

            //assert
            _bucketServiceMock.Verify(
                v => v.BackupFilesToBucket(settingsFolder, _bucketName, folderPath), Times.Once);

        }
    }
}
