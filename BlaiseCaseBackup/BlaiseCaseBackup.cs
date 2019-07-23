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
using System.Timers;
using System.Globalization;
using StatNeth.Blaise.API.DataLink;
using StatNeth.Blaise.API.ServerManager;

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
            RunBackup();
        }

        protected override void OnStart(string[] args)
        {
            // Get the MinuteRunTimer env variable and convert it from minutes to miliseconds.
            string timerString = ConfigurationManager.AppSettings["MinuteRunTimer"];
            double time = double.Parse(timerString, CultureInfo.InvariantCulture.NumberFormat);
            time = time * 60 * 1000;

            // Set up a timer
            Timer timer = new Timer();
            timer.Interval = time;
            timer.Elapsed += new ElapsedEventHandler(this.StartBackup);
            timer.Start();
        }

        protected override void OnStop()
        {
            log.Info("Blaise Case Backup service stopped.");
        }

        public void StartBackup(object sender, ElapsedEventArgs args)
        {
            log.Info("Starting backup functionality...");
            RunBackup();
        }

        /// <summary>
        /// Sets up and runs the backup process. This function connects to the local Blaise Server
        /// Manager and collects a list of the connected server parks. Each one is then 
        /// backed up through a process that loops through the collected server parks, running the
        /// back up functionality.
        /// </summary>
        public void RunBackup()
        {
            log.Info("Running backup process...");
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
                    BackupSurvey(serverPark, survey);
                }
            }
        }

        /// <summary>
        /// Runs a series of functions necessary for backing up a Blaise data file.
        /// </summary>
        /// <param name="serverPark">The server park on which the target survey resides.</param>
        /// <param name="instrument">The name of the survey (instrument).</param>
        public void BackupSurvey(ServerManagerAPI.IServerPark serverPark, ServerManagerAPI.ISurvey instrument)
        {

            log.Info(String.Format("Survey found - {0}/{1}.", serverPark.Name, instrument.Name));

            // Get the BMI and BDI files for the survey:
            string originalBDI = GetDataFileName(serverPark.Name, instrument.Name);
            string originalBMI = GetMetaFileName(serverPark.Name, instrument.Name);

            if (originalBDI == "" || originalBMI == "")
                return;

            // Use one of the files to get their directory: 
            string directory = Path.GetDirectoryName(originalBDI);
            string backupBDI = CreateBackupFile(instrument.Name, directory, originalBDI, originalBMI);

            if (backupBDI == null)
            {
                log.Error(String.Format("Backup BDI not available - {0}/{1} aborted.", serverPark.Name, instrument.Name));
                return;
            }
            // Get data links for the original and the backup data interfaces:
            //var originalDataLink = GetDataLinkFromBDI(originalBDI);
            var originalDataLink = GetRemoteDataLink(serverPark, instrument);

            var backupDataLink = GetDataLinkFromBDI(backupBDI);

            if (originalDataLink == null || backupDataLink == null)
            {
                log.Error(String.Format("Backup of {0}/{1} aborted.", serverPark.Name, instrument.Name));
                log.Error("One of the two datalink objects has returned NULL.");
                log.Error("Original : " + originalDataLink);
                log.Error("Backup : " + backupDataLink);
                return;
            }
            log.Info("Datalink connection established: Record Count : " + originalDataLink.RecordCount);

            bool copySuccess = CopyDataRecords(originalDataLink, backupDataLink);
            if (!copySuccess)
            {
                log.Error(String.Format("Error backing up Blaise data - {0}/{1}", serverPark.Name, instrument.Name));
                return;
            }

            string[] filesToMove = { instrument.Name + "_BACKUP.bdix", instrument.Name + "_BACKUP.bdbx" };

            //Drop the datalink objects out of scope to free up the resources.
            originalDataLink = null;
            backupDataLink = null;

            bool success = MoveDataToFolder(instrument.Name, directory, "C:\\BlaiseBackup", filesToMove);

            if (success)
                log.Info(String.Format("Successfully backed up Blaise data - {0}/{1}", serverPark.Name, instrument.Name));
            else
                log.Error(String.Format("Error backing up Blaise data - {0}/{1}", serverPark.Name, instrument.Name));
        }

        /// <summary>
        /// Gets the name of the data file (.bdix) associated with a specified serverpark and instrument.
        /// </summary>
        /// <param name="serverPark">The serverpark where the instrument exists.</param>
        /// <param name="instrument">The instrument who's data file we're getting.</param>
        /// <returns>The string name of the data file (.bdix)</returns>
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

        /// <summary>
        /// Gets the name of the meta file (.bmix) associated with a specified serverpark and instrument.
        /// </summary>
        /// <param name="serverPark">The serverpark where the instrument exists.</param>
        /// <param name="instrument">The instrument who's meta file we're getting.</param>
        /// <returns>The string name of the meta file (.bmix)</returns>
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

        /// <summary>
        /// Generates a backup data file from the target data (.bdix) and meta (.bmix) files provided.
        /// </summary>
        /// <param name="instrument">The name of the survey (instrument)</param>
        /// <param name="directory">The directory where the source files reside.</param>
        /// <param name="bdixFileName">The name of the source data file (.bdix)</param>
        /// <param name="bmixFileName">The name of the source meta file (.bmix)</param>
        /// <returns></returns>
        public string CreateBackupFile(string instrument, string directory, string bdixFileName, string bmixFileName)
        {
            try
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

                // Reset the datasource reference to be in local space.
                csb.DataSource = instrument + "_BACKUP.bdbx";
                di.ConnectionInfo.SetConnectionString(csb.ConnectionString);

                di.CreateDatabaseObjects(null, true);
                di.SaveToFile(true);

                return di.FileName;
            }
            catch (Exception e)
            {
                log.Error("Error creating backup objects.");
                log.Error(e.Message);
                log.Error(e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Gets and returns a datalink connection to the target data file (.bdix).
        /// </summary>
        /// <param name="bdiFile">The name of the target data file (.bdix).</param>
        /// <returns>A IDatalink connection object user to access the stored Blaise data.</returns>
        public IDataLink GetDataLinkFromBDI(string bdiFile)
        {
            try
            {
                var dl = DataLinkAPI.DataLinkManager.GetDataLink(bdiFile);
                if (dl.RecordCount.GetType() == typeof(int))
                {
                    return dl;
                }
                else
                {
                    log.Error("Error Getting DataLink");
                    log.Error("Invalid datalink type : " + dl.RecordCount.GetType());
                    log.Error("Datalink value : " + dl.RecordCount);
                    return null;
                }

            }
            catch(Exception e)
            {
                log.Error("Error Getting DataLink");
                log.Error(e.Message);
                log.Error(e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Uses two datalink objects to copy the stored data from the original to the backup datastore.
        /// </summary>
        /// <param name="originalDL">A datalink object referencing the source data location.</param>
        /// <param name="backupDL">A datalink object referencing the backup data location.</param>
        public bool CopyDataRecords(IDataLink originalDL, IDataLink backupDL)
        {
            try
            {
                IDataSet ds = originalDL.Read("");

                while (!ds.EndOfSet)
                {
                    // Read the current record and write it to the backup database:
                    var dr = ds.ActiveRecord;
                    if(dr != null)
                        backupDL.Write(dr);

                    // Move to the next record:
                    ds.MoveNext();
                }
                return true;
            }
            catch (Exception e)
            {
                log.Error("Error Copying data records.");
                log.Error(e.Message);
                log.Error(e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Moves a data file/collection of data files from one directory to a backup directory 
        /// using the instrument name as a sub folder.
        /// </summary>
        /// <param name="instrument">Name and version of the current survey (eg. OPN1901A).</param>
        /// <param name="sourceDirectory">The directory from which the file(s) will be collected.</param>
        /// <param name="backupDirectory">The directory where the new file subfolder will be created.</param>
        /// <param name="files">The string name of each file being moved.</param>
        /// <returns>A boolean indicating the success of the function.</returns>
        public bool MoveDataToFolder(string instrument, string sourceDirectory, string backupDirectory, string[] files)
        {
            try
            {
                Directory.GetAccessControl(sourceDirectory);
                Directory.CreateDirectory(backupDirectory);
                foreach (string fileName in files)
                {
                    string targetDirectory = backupDirectory + "\\" + instrument;
                    Directory.CreateDirectory(targetDirectory);

                    string sourceFilePath = sourceDirectory + "\\" + fileName;
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

        /// <summary>
        /// Method for connecting to Blaise data sets.
        /// </summary>
        /// /// <param name="hostname">The name of the hostname.</param>
        /// <param name="instrumentName">The name of the instrument.</param>
        /// <param name="serverPark">The name of the server park.</param>
        /// <returns> IDataLink4 object for the connected server park.</returns>
        public static IDataLink4 GetRemoteDataLink(ServerManagerAPI.IServerPark serverPark, ServerManagerAPI.ISurvey instrument)
        {
            string serverName = ConfigurationManager.AppSettings["BlaiseServerHostName"];
            string userName = ConfigurationManager.AppSettings["BlaiseServerUserName"];
            string password = ConfigurationManager.AppSettings["BlaiseServerPassword"];

            // Get the GIID of the instrument.
            Guid instrumentID = Guid.NewGuid();
            try
            {
                instrumentID = instrument.InstrumentID;

                // Connect to the data.
                IRemoteDataServer dataLinkConn = DataLinkManager.GetRemoteDataServer(serverName, 8033, userName, GetPassword(password));

                return dataLinkConn.GetDataLink(instrumentID, serverPark.Name);
            }
            catch (Exception e)
            {
                log.Error("Error connecting to remote data link.");
                log.Error(e.Message);
                log.Error(e.StackTrace);
                return null;
            }
        }

    }
}
