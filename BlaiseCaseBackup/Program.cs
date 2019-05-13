using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace BlaiseCaseBackup
{
    static class Program
    {


        // Instantiate logger.
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main()
        {
#if DEBUG

            log.Info("Blaise Case Creator service starting in DEBUG mode.");
            BlaiseCaseBackup bcbService = new BlaiseCaseBackup();
            bcbService.OnDebug();
#else
            log.Info("Blaise Case Creator service starting in RELEASE mode.");
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
