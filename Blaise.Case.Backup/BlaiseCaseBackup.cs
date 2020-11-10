using System.ServiceProcess;
using Blaise.Case.Backup.WindowsService.Interfaces;
using Blaise.Case.Backup.WindowsService.Ioc;

namespace Blaise.Case.Backup.WindowsService
{
    public partial class BlaiseCaseBackup : ServiceBase
    {
        public IInitialiseWindowsService InitialiseService;

        public BlaiseCaseBackup()
        {
            InitializeComponent();
            var unityProvider = new UnityProvider();

            InitialiseService = unityProvider.Resolve<IInitialiseWindowsService>();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            InitialiseService.Start();
        }

        protected override void OnStop()
        {
            InitialiseService.Stop();
        }
    }
}
