using System.ServiceProcess;

namespace Blaise.Case.Backup
{
    static class Program
    {
        // Instantiate logger.
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main()
        {
#if DEBUG

            Logger.Info("Blaise Case Backup service starting in DEBUG mode.");
            BlaiseCaseBackup bcbService = new BlaiseCaseBackup();
            bcbService.OnDebug();
#else
            Logger.Info("Blaise Case Backup service starting in RELEASE mode.");
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new BlaiseCaseBackup()
            };
            ServiceBase.Run(ServicesToRun);
       
#endif
        }
    }
}
