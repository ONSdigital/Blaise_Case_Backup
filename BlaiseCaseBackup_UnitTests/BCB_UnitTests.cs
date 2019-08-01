using System;
using System.Configuration;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlaiseCaseBackup_UnitTests
{
    [TestClass]
    public class BCB_Tests
    {
        [TestMethod]
        public void Test_Get_Data_File_Name()
        {
            var b = new BlaiseCaseBackup.BlaiseCaseBackup();
            string dfName = b.GetDataFileName("LocalDevelopment", "OPN1901A");

            Assert.AreNotEqual("", dfName);
        }

        [TestMethod]
        public void Test_Get_Data_File_Name_Fail_SP()
        {
            var b = new BlaiseCaseBackup.BlaiseCaseBackup();
            string dfName = b.GetDataFileName("BadServerPark", "HealthSurvey");

            Assert.AreEqual("", dfName);
        }

        [TestMethod]
        public void Test_Get_Data_File_Name_Fail_Instrument()
        {
            var b = new BlaiseCaseBackup.BlaiseCaseBackup();
            string dfName = b.GetDataFileName("LocalDevelopment", "BadInstrument");

            Assert.AreEqual("", dfName);
        }

        [TestMethod]
        public void Test_Get_Meta_File_Name()
        {
            var b = new BlaiseCaseBackup.BlaiseCaseBackup();
            string mfName = b.GetMetaFileName("LocalDevelopment", "OPN1901A");

            Assert.AreNotEqual("", mfName);
        }

        [TestMethod]
        public void Test_Get_Meta_File_Name_Fail_SP()
        {
            var b = new BlaiseCaseBackup.BlaiseCaseBackup();
            string mfName = b.GetMetaFileName("BadServerPark", "HealthSurvey");

            Assert.AreEqual("", mfName);
        }

        [TestMethod]
        public void Test_Get_Meta_File_Name_Fail_Instrument()
        {
            var b = new BlaiseCaseBackup.BlaiseCaseBackup();
            string mfName = b.GetMetaFileName("LocalDevelopment", "BadInstrument");

            Assert.AreEqual("", mfName);
        }

        [TestMethod]
        public void Test_Get_Connection()
        {
            string serverName = ConfigurationManager.AppSettings.Get("BlaiseServerHostName");
            string username = ConfigurationManager.AppSettings.Get("BlaiseServerUserName");
            string password = ConfigurationManager.AppSettings.Get("BlaiseServerPassword");
            string binding = ConfigurationManager.AppSettings["BlaiseServerBinding"];

            var b = new BlaiseCaseBackup.BlaiseCaseBackup();
            var connection = b.ConnectToBlaiseServer(serverName, username, password, binding);

            Assert.AreNotEqual(null, connection);
        }

        [TestMethod]
        public void Test_Get_Connection_Fail_Password()
        {
            string serverName = ConfigurationManager.AppSettings.Get("BlaiseServerHostName");
            string username = ConfigurationManager.AppSettings.Get("BlaiseServerUserName");
            string password = "BadPassword";
            string binding = ConfigurationManager.AppSettings["BlaiseServerBinding"];

            var b = new BlaiseCaseBackup.BlaiseCaseBackup();
            var connection = b.ConnectToBlaiseServer(serverName, username, password, binding);

            Assert.AreEqual(null, connection);
        }

        [TestMethod]
        public void Test_Get_Connection_Fail_User()
        {
            string serverName = ConfigurationManager.AppSettings.Get("BlaiseServerHostName");
            string username = "BadUser";
            string password = ConfigurationManager.AppSettings.Get("BlaiseServerPassword");
            string binding = ConfigurationManager.AppSettings["BlaiseServerBinding"];

            var b = new BlaiseCaseBackup.BlaiseCaseBackup();
            var connection = b.ConnectToBlaiseServer(serverName, username, password, binding);

            Assert.AreEqual(null, connection);
        }

        [TestMethod]
        public void Test_Get_Connection_Fail_Server()
        {
            string serverName = "BadServer";
            string username = ConfigurationManager.AppSettings.Get("BlaiseServerUserName");
            string password = ConfigurationManager.AppSettings.Get("BlaiseServerPassword");
            string binding = ConfigurationManager.AppSettings["BlaiseServerBinding"];

            var b = new BlaiseCaseBackup.BlaiseCaseBackup();
            var connection = b.ConnectToBlaiseServer(serverName, username, password, binding);

            Assert.AreEqual(null, connection);
        }

        [TestMethod]
        public void Test_Get_Password()
        {
            string password = "Password";

            var securePassword = BlaiseCaseBackup.BlaiseCaseBackup.GetPassword(password);

            Assert.AreEqual(8, securePassword.Length);
        }
        
        [TestMethod]
        public void Test_Move_File()
        {
            var b = new BlaiseCaseBackup.BlaiseCaseBackup();

            string currentDir = Directory.GetCurrentDirectory() + "\\MoveFolder";

            if (!Directory.Exists(currentDir))
            {
                Directory.CreateDirectory(currentDir);
            }

            string[] files = { "testFile1.txt", "testFile2.txt" };

            foreach (string file in files)
            {
                Console.Out.WriteLine(currentDir);
                var f = File.Create(currentDir + "\\" + file);
                f.Close();
            }

            bool success = b.MoveDataToFolder("test", currentDir, currentDir, files);

            Assert.AreEqual(true, success);
        }

        [TestMethod]
        public void Test_Move_File_Fail_NoFile()
        {
            var b = new BlaiseCaseBackup.BlaiseCaseBackup();

            string currentDir = Directory.GetCurrentDirectory() + "\\MoveFolder";

            string[] files = { "testFile1.txt", "testFile2.txt" };

            bool success = b.MoveDataToFolder("test", currentDir, currentDir, files);

            Assert.AreEqual(false, success);
        }

        [TestMethod]
        public void Test_Get_Datalink_From_BDI()
        {
            var b = new BlaiseCaseBackup.BlaiseCaseBackup();

            string serverPark = "LocalDevelopment";
            string instrument = "OPN1901A";
            // Get the BMI and BDI files for the survey:
            string originalBDI = b.GetDataFileName(serverPark, instrument);


            // Get data links for the original and the backup data interfaces:
            var originalDataLink = b.GetDataLinkFromBDI(originalBDI);

            Assert.AreNotEqual(null, originalDataLink);
        }

        [TestMethod]
        public void Test_Get_Datalink_From_BDI_Fail_SP()
        {
            var b = new BlaiseCaseBackup.BlaiseCaseBackup();

            string serverPark = "BadServerpark";
            string instrument = "HealthSurvey";
            // Get the BMI and BDI files for the survey:
            string originalBDI = b.GetDataFileName(serverPark, instrument);


            // Get data links for the original and the backup data interfaces:
            var originalDataLink = b.GetDataLinkFromBDI(originalBDI);

            Assert.AreEqual(null, originalDataLink);
        }

        [TestMethod]
        public void Test_Get_Datalink_From_BDI_Fail_Instrument()
        {
            var b = new BlaiseCaseBackup.BlaiseCaseBackup();

            string serverPark = "LocalDevelopment";
            string instrument = "BadInstrument";
            // Get the BMI and BDI files for the survey:
            string originalBDI = b.GetDataFileName(serverPark, instrument);


            // Get data links for the original and the backup data interfaces:
            var originalDataLink = b.GetDataLinkFromBDI(originalBDI);

            Assert.AreEqual(null, originalDataLink);
        }
    }
}
