using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.ServiceProcess;
using DataInterfaceAPI = StatNeth.Blaise.API.DataInterface;
using ServerManagerAPI = StatNeth.Blaise.API.ServerManager;
using DataLinkAPI = StatNeth.Blaise.API.DataLink;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using log4net;

namespace BlaiseCaseBackup
{
    public partial class BlaiseCaseBackup : ServiceBase
    {
        // Instantiate logger.
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public BlaiseCaseBackup()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            this.OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            RunBackup();
        }

        public void RunBackup()
        {
            // Connection parameters
            string serverName = ConfigurationManager.AppSettings["BlaiseServerHostName"];
            string userName = ConfigurationManager.AppSettings["BlaiseServerUserName"];
            string password = ConfigurationManager.AppSettings["BlaiseServerPassword"];

            ServerManagerAPI.IConnectedServer serverManagerConnection = null;
            try
            {
                log.Info("Attempting to connect to Blaise Server Manager.");
                serverManagerConnection = ServerManagerAPI.ServerManager.ConnectToServer(serverName, 8031, userName, GetPassword(password));
            }
            catch (Exception e)
            {
                log.Error("Error connecting to Blaise Server Manager.");
                log.Error(e.Message);
                log.Error(e.StackTrace);
            }

            // Loop through the server parks on the connected Blaise server.
            foreach (ServerManagerAPI.IServerPark serverPark in serverManagerConnection.ServerParks)
            {
                // Loop through the surveys installed on the current server park
                foreach (ServerManagerAPI.ISurvey survey in serverManagerConnection.GetServerPark(serverPark.Name).Surveys)
                {
                    BackupSurvey(serverPark.Name, survey.Name);
                }
            }
        }

        protected override void OnStop()
        {
        }

        public void BackupSurvey(string serverPark, string instrument)
        {
            // Get the BMI and BDI files for the survey:
            string originalBDI = GetDataFileName(serverPark, instrument);
            string originalBMI = GetMetaFileName(serverPark, instrument);

            // Use one of the files to get their directory: 
            string directory = Path.GetDirectoryName(originalBDI);
            string backupBDI = CreateBackupFile(instrument, directory, originalBDI, originalBMI);

            // Get data links for the original and the backup data interfaces:
            var originalDataLink = GetDataLinkFromBDI(originalBDI);
            var backupDataLink = GetDataLinkFromBDI(backupBDI);

            CopyDataRecords(originalDataLink, backupDataLink);

            string[] filesToMove = { instrument + "_BACKUP.bdix", instrument + "_BACKUP.bdbx" };

            originalDataLink = null;
            backupDataLink = null;


            MoveDataToFolder(instrument, directory, "C:\\BlaiseBackup", filesToMove);
        }

        public string GetDataFileName(string serverPark, string instrument)
        {
            try
            {
                string serverName = ConfigurationManager.AppSettings.Get("BlaiseServerHostName");
                string username = ConfigurationManager.AppSettings.Get("BlaiseServerUserName");
                string password = ConfigurationManager.AppSettings.Get("BlaiseServerPassword");

                var connection = ConnectToBlaiseServer(serverName, username, password);


                var surveys = connection.GetSurveys(serverPark);

                foreach (ServerManagerAPI.ISurvey2 survey in surveys)
                {
                    if (survey.Name == instrument)
                    {
                        var conf = survey.Configuration.Configurations.ElementAt(0);

                        return conf.DataFileName;
                    }
                }

                return "";
            }
            catch (Exception e)
            {
                log.Error("Error getting meta file name.");
                log.Error(e.Message);
                log.Error(e.StackTrace);

                return "";
            }
        }

        public string GetMetaFileName(string serverPark, string instrument)
        {
            try
            {
                string serverName = ConfigurationManager.AppSettings.Get("BlaiseServerHostName");
                string username = ConfigurationManager.AppSettings.Get("BlaiseServerUserName");
                string password = ConfigurationManager.AppSettings.Get("BlaiseServerPassword");

                var connection = ConnectToBlaiseServer(serverName, username, password);

                var surveys = connection.GetSurveys(serverPark);

                foreach (ServerManagerAPI.ISurvey2 survey in surveys)
                {
                    if (survey.Name == instrument)
                    {
                        var conf = survey.Configuration.Configurations.ElementAt(0);

                        return conf.MetaFileName;
                    }
                }

                return "";
            }
            catch(Exception e)
            {
                log.Error("Error getting meta file name.");
                log.Error(e.Message);
                log.Error(e.StackTrace);

                return "";
            }
        }

        public string CreateBackupFile(string instrument, string directory, string bdixFileName, string bmixFileName)
        {
            // Get an empty IDataInterface:
            DataInterfaceAPI.IDataInterface di = DataInterfaceAPI.DataInterfaceManager.GetDataInterface();

            // Fill the ConnectionInfo:
            di.ConnectionInfo.DataSourceType = DataInterfaceAPI.DataSourceType.Blaise;
            di.ConnectionInfo.DataProviderType = DataInterfaceAPI.DataProviderType.BlaiseDataProviderForDotNET;

            //SpecifyDataPartitionType:
            di.DataPartitionType = DataInterfaceAPI.DataPartitionType.Stream;

            // Create a connection string using IBlaiseConnectionStringBuilder
            DataInterfaceAPI.IBlaiseConnectionStringBuilder csb = DataInterfaceAPI.DataInterfaceManager.GetBlaiseConnectionStringBuilder();
            csb.DataSource = directory + "\\" + instrument + "_BACKUP.bdbx";

            di.ConnectionInfo.SetConnectionString(csb.ConnectionString);

            // Specify file name of data model and bdix:
            di.DatamodelFileName = bmixFileName;
            di.FileName = directory + "\\" + instrument + "_BACKUP.bdix";
            // Create table definitions and database objects:
            di.CreateTableDefinitions();
            di.CreateDatabaseObjects(null, true);
            di.SaveToFile(true);

            return di.FileName;
        }

        public DataLinkAPI.IDataLink GetDataLinkFromBDI(string bdiFile)
        {
            try
            {
                var dl = DataLinkAPI.DataLinkManager.GetDataLink(bdiFile);
                return dl;
            }
            catch
            {
                Console.WriteLine("Error Getting DataLink");
                return null;
            }
        }

        public void CopyDataRecords(DataLinkAPI.IDataLink originalDL, DataLinkAPI.IDataLink backupDL)
        {
            DataLinkAPI.IDataSet ds = originalDL.Read("");

            while (!ds.EndOfSet)
            {
                // Read the current record and write it to the backup database:
                var dr = ds.ActiveRecord;
                backupDL.Write(dr);

                // Move to the next record:
                ds.MoveNext();
            }
        }


        public bool MoveDataToFolder(string instrument, string currentDirectory, string backupDirectory, string[] files)
        {
            try
            {
                Directory.GetAccessControl(currentDirectory);
                Directory.CreateDirectory(backupDirectory);
                foreach (string fileName in files)
                {
                    string targetDirectory = backupDirectory + "\\" + instrument;
                    Directory.CreateDirectory(targetDirectory);

                    string sourceFilePath = currentDirectory + "\\" + fileName;
                    string destFilePath = targetDirectory + "\\" + fileName;

                    // Check if the backup file exists, delete it if it does.
                    if (File.Exists(destFilePath))
                        File.Delete(destFilePath);

                    Directory.Move(sourceFilePath, destFilePath);
                }
                return true;
            }
            catch(Exception e)
            {
                log.Error("Error moving backup data file.");
                log.Error(e.Message);
                log.Error(e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Establishes a connection to a Blaise Server.
        /// </summary>
        /// <param name="serverName">The location of the Blaise server.</param>
        /// <param name="userName">Username with access to the specified server.</param>
        /// <param name="password">Password for the specified user to access the server.</param>
        /// <returns>A IConnectedServer2 object which is connected to the server provided.</returns>
        public ServerManagerAPI.IConnectedServer2 ConnectToBlaiseServer(string serverName, string userName, string password)
        {
            int port = 8031;
            try
            {
                ServerManagerAPI.IConnectedServer2 connServer =
                    (ServerManagerAPI.IConnectedServer2)ServerManagerAPI.ServerManager.ConnectToServer(serverName, port, userName, GetPassword(password));

                return connServer;
            }
            catch (Exception e)
            {
                log.Error("Error getting Blaise connection.");
                log.Error(e.Message);
                log.Error(e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Converts a string password to the SecureString format.
        /// </summary>
        /// <param name="pw">The string password being read in for conversion.</param>
        /// <returns>A SecureString version of the imported password.</returns>
        public static SecureString GetPassword(string pw)
        {
            char[] passwordChars = pw.ToCharArray();
            SecureString password = new SecureString();
            foreach (char c in passwordChars)
            {
                password.AppendChar(c);
            }
            return password;
        }

    }
}
