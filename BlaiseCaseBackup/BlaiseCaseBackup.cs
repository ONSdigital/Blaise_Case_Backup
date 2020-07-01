using System.ServiceProcess;
using Blaise.Nuget.Api;
using Blaise.Nuget.Api.Contracts.Interfaces;
using BlaiseCaseBackup.Interfaces;
using BlaiseCaseBackup.Providers;
using BlaiseCaseBackup.Services;
using log4net;
using Unity;

namespace BlaiseCaseBackup
{
    public partial class BlaiseCaseBackup : ServiceBase
    {
        public IInitialiseService InitialiseService;

        public BlaiseCaseBackup()
        {
            InitializeComponent();
            IUnityContainer unityContainer = new UnityContainer();

            unityContainer.RegisterType<IConfigurationProvider, ConfigurationProvider>();

            //blaise services
            unityContainer.RegisterType<IFluentBlaiseApi, FluentBlaiseApi>();

            //logging
            unityContainer.RegisterFactory<ILog>(f => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType));

            //services
            unityContainer.RegisterType<IBackupSurveysService, BackupSurveysService>();

            //main service
            unityContainer.RegisterType<IInitialiseService, InitialiseService>();

            //resolve all dependencies as CaseCreationService is the entry point
            InitialiseService = unityContainer.Resolve<IInitialiseService>();
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
