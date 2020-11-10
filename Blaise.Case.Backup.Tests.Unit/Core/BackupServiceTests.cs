using System.Collections.Generic;
using Blaise.Case.Backup.CloudStorage.Interfaces;
using Blaise.Case.Backup.Core;
using Blaise.Case.Backup.Core.Interfaces;
using Blaise.Case.Backup.Data.Interfaces;
using log4net;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Case.Backup.Tests.Unit.Core
{
    public class BackupServiceTests
    {
        private Mock<ILog> _loggingMock;
        private Mock<IConfigurationProvider> _configurationProviderMock;
        private Mock<IBlaiseApiService> _blaiseApiServiceMock;
        private Mock<IStorageService> _storageServiceMock;

        private readonly string _instrumentName;
        private readonly string _serverParkName;
        private readonly string _bucketName;
        private readonly string _localBackupPath;
        private readonly string _vmName;

        public BackupServiceTests()
        {
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

            _blaiseApiServiceMock = new Mock<IBlaiseApiService>();

            _storageServiceMock = new Mock<IStorageService>();

            _sut = new BackupService(
                _loggingMock.Object,
                _configurationProviderMock.Object,
                _blaiseApiServiceMock.Object,
                _storageServiceMock.Object);              
        }

        [Test]
        public void Given_I_Call_BackupSurveys_And_There_Are_No_Surveys_Then_No_Records_Are_Processed()
        {
            //arrange
            _blaiseApiServiceMock.Setup(b => b.GetAvailableSurveys()).Returns(new List<ISurvey>());

            //act
            _sut.BackupSurveys();

            //assert
            _blaiseApiServiceMock.Verify(v => v.GetAvailableSurveys(), Times.Once);

            _blaiseApiServiceMock.Verify(v => v.BackupSurveyToFile(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            _storageServiceMock.Verify(v => v.BackupFilesToBucket(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void Given_I_Call_BackupSurveys_And_There_Are_No_Surveys_Then_I_Log_A_Warning()
        {
            //arrange
            _blaiseApiServiceMock.Setup(b => b.GetAvailableSurveys()).Returns(new List<ISurvey>());

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

            _blaiseApiServiceMock.Setup(b => b.GetAvailableSurveys()).Returns(new List<ISurvey> { surveyMock.Object });

            var localFolderPath = $"{_localBackupPath}/{_serverParkName}";
            var folderPath = $"{_vmName}/{_serverParkName}";

            //act
            _sut.BackupSurveys();

            //assert
            _blaiseApiServiceMock.Verify(v => v.GetAvailableSurveys(), Times.Once);

            _blaiseApiServiceMock.Verify(v => v.BackupSurveyToFile(_serverParkName,
                _instrumentName, localFolderPath), Times.Once);

            _storageServiceMock.Verify(v => v.BackupFilesToBucket(localFolderPath,
                _bucketName,  folderPath), Times.Once);
        }

        [Test]
        public void Given_I_Call_BackupSettings_And_There_Are_Settings_Files_Then_The_Files_Are_Backed_Up()
        {
            //arrange
            var settingsFolder = "SettingsFolder";
            var folderPath = $"{_vmName}/Settings";

            _configurationProviderMock.Setup(c => c.SettingsFolder).Returns(settingsFolder);

            _blaiseApiServiceMock.Setup(b => b.BackupSurveyToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            //act
            _sut.BackupSettings();

            //assert
            _storageServiceMock.Verify(
                v => v.BackupFilesToBucket(settingsFolder, _bucketName, folderPath), Times.Once);

        }
    }
}
