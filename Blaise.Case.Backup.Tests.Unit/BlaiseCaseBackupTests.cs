using Blaise.Case.Backup.Interfaces;
using NUnit.Framework;

namespace Blaise.Case.Backup.Tests.Unit
{
    public class BlaiseCaseBackupTests
    {
        [Test]
        public void Given_I_Create_A_New_Instance_Of_BlaiseCaseCreator_Then_No_Exceptions_Are_Thrown()
        {
            //act && assert
            // ReSharper disable once ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new BlaiseCaseBackup());
        }

        [Test]
        public void
            Given_I_Create_A_New_Instance_Of_BlaiseCaseCreator_Then_All_Dependencies_Are_Registered_And_Resolved()
        {
            //arrange

            //act
            var result = new BlaiseCaseBackup();

            //assert
            Assert.NotNull(result.InitialiseService);
            Assert.IsInstanceOf<IInitialiseService>(result.InitialiseService);
        }
    }
}
