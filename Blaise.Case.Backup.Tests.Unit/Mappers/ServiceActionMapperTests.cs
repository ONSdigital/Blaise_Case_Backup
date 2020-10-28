using Blaise.Case.Backup.Enums;
using Blaise.Case.Backup.Mappers;
using Blaise.Case.Backup.Models;
using NUnit.Framework;

namespace Blaise.Case.Backup.Tests.Unit.Mappers
{
    public class ServiceActionMapperTests
    {
        private readonly ServiceActionMapper _sut;

        public ServiceActionMapperTests()
        {
            _sut = new ServiceActionMapper();
        }


        [Test]
        public void Given_A_Message_With_An_Inspect_Action_When_I_Call_MapToCaseMonitorActionModel_Then_I_Get_An_Expected_Model_Back()
        {
            //arrange
            const string message =
                @"{ ""action"":""start_backup""}";

            //act
            var result = _sut.MapToCaseBackupActionModel(message);

            //assert`
            Assert.NotNull(result);
            Assert.IsInstanceOf<CaseBackupActionModel>(result);
            Assert.AreEqual(ActionType.StartBackup, result.Action);
        }

        [Test]
        public void Given_An_Invalid_Message_When_I_Call_MapToCaseMonitorActionModel_Then_I_Get_A_Default_Model_Back()
        {
            //arrange
            const string message =
                @"{ ""ACTION"":""none""}";

            //act
            var result = _sut.MapToCaseBackupActionModel(message);

            //assert`
            Assert.NotNull(result);
            Assert.IsInstanceOf<CaseBackupActionModel>(result);
            Assert.AreEqual(ActionType.NotSupported, result.Action);
        }
    }
}
