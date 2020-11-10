using System.Collections.Generic;
using System.Linq;
using Blaise.Case.Backup.Data;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Case.Backup.Tests.Unit.Data
{
    public class BlaiseApiServiceTests
    {
        private Mock<IBlaiseApi> _blaiseApiMock;

        private readonly ConnectionModel _connectionModel;

        public BlaiseApiServiceTests()
        {
            _connectionModel = new ConnectionModel();
        }

        private BlaiseApiService _sut;

        [SetUp]
        public void SetUpTests()
        {
            _blaiseApiMock = new Mock<IBlaiseApi>();
            _blaiseApiMock.Setup(b => b.GetDefaultConnectionModel()).Returns(_connectionModel);


            _sut = new BlaiseApiService(_blaiseApiMock.Object);              
        }
        
        [Test]
        public void Given_There_Are_Surveys_Available_When_I_Call_GetAvailableSurveys_Then_The_Correct_Method_Is_Called()
        {
            //arrange
 
            _blaiseApiMock.Setup(b => b.GetAllSurveys(_connectionModel));

            //act
            _sut.GetAvailableSurveys();

            //assert
            _blaiseApiMock.Verify(
                v => v.GetAllSurveys(_connectionModel), Times.Once);
        }

        [Test]
        public void Given_There_Are_Surveys_Available_When_I_Call_GetAvailableSurveys_Then_The_Correct_Surveys_Are_Returned()
        {
            //arrange
            var survey1Name = "Survey1";
            var survey2Name = "Survey2";
            var survey3Name = "Survey3";

            var survey1Mock = new Mock<ISurvey>();
            survey1Mock.Setup(s => s.Name).Returns(survey1Name);

            var survey2Mock = new Mock<ISurvey>();
            survey2Mock.Setup(s => s.Name).Returns(survey2Name);

            var survey3Mock = new Mock<ISurvey>();
            survey3Mock.Setup(s => s.Name).Returns(survey3Name);

            var surveys = new List<ISurvey> { survey1Mock.Object, survey2Mock.Object, survey3Mock.Object };

            _blaiseApiMock.Setup(b => b.GetAllSurveys(_connectionModel)).Returns(surveys);

            //act
            var result =_sut.GetAvailableSurveys();

            //assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Any(r => r.Name == survey1Name));
            Assert.IsTrue(result.Any(r => r.Name == survey2Name));
            Assert.IsTrue(result.Any(r => r.Name == survey3Name));
        }

        [Test]
        public void Given_There_Are_No_Surveys_Available_When_I_Call_GetAvailableSurveys_Then_An_Empty_List_Is_Returned()
        {
            //arrange

            var surveys = new List<ISurvey>();

            _blaiseApiMock.Setup(b => b.GetAllSurveys(_connectionModel)).Returns(surveys);

            //act
            var result = _sut.GetAvailableSurveys();

            //assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void Given_Valid_Parameters_When_I_Call_BackupSurveyToFile_Then_The_Correct_Method_Is_Called()
        {
            //arrange
            var serverPark = "serverPark1";
            var instrumentName = "instrument1";
            var outputPath = "Path";

            _blaiseApiMock.Setup(b => b.GetAllSurveys(_connectionModel));

            //act
            _sut.BackupSurveyToFile(serverPark, instrumentName, outputPath);

            //assert
            _blaiseApiMock.Verify(
                v => v.BackupSurveyToFile(_connectionModel, serverPark, instrumentName, outputPath), Times.Once);
        }
    }
}
