using Blaise.Case.Backup.MessageBroker.Mappers;
using Blaise.Case.Backup.MessageBroker.Models;
using NUnit.Framework;
using Blaise.Case.Backup.MessageBroker.Enums;

namespace Blaise.Case.Backup.Tests.Unit.MessageBroker.Mappers
{
    public class MessageModelMapperTests
    {
        private readonly MessageModelMapper _sut;

        public MessageModelMapperTests()
        {
            _sut = new MessageModelMapper();
        }


        [Test]
        public void Given_A_Message_With_An_Inspect_Action_When_I_Call_MapToCaseMonitorActionModel_Then_I_Get_An_Expected_Model_Back()
        {
            //arrange
            const string message =
                @"{ ""action"":""start_backup""}";

            //act
            var result = _sut.MapToMessageModel(message);

            //assert`
            Assert.NotNull(result);
            Assert.IsInstanceOf<MessageModel>(result);
            Assert.AreEqual(ActionType.StartBackup, result.Action);
        }

        [Test]
        public void Given_An_Invalid_Message_When_I_Call_MapToCaseMonitorActionModel_Then_I_Get_A_Default_Model_Back()
        {
            //arrange
            const string message =
                @"{ ""ACTION"":""none""}";

            //act
            var result = _sut.MapToMessageModel(message);

            //assert`
            Assert.NotNull(result);
            Assert.IsInstanceOf<MessageModel>(result);
            Assert.AreEqual(ActionType.NotSupported, result.Action);
        }
    }
}
