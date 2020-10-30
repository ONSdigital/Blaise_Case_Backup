using System.ServiceProcess;
using Blaise.Case.Backup.Interfaces;
using Blaise.Case.Backup.Providers;

namespace Blaise.Case.Backup
{
    public partial class BlaiseCaseBackup : ServiceBase
    {
        public IInitialiseService InitialiseService;

        public BlaiseCaseBackup()
        {
            InitializeComponent();
            var unityProvider = new UnityProvider();

            InitialiseService = unityProvider.Resolve<IInitialiseService>();
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
