using System;
using System.Configuration;
using System.ServiceProcess;
using Blaise.Nuget.Api;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.PubSub.Api;
using Blaise.Nuget.PubSub.Contracts.Interfaces;
using BlaiseCaseBackup.Interfaces;
using BlaiseCaseBackup.Mappers;
using BlaiseCaseBackup.MessageHandler;
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
            unityContainer.RegisterSingleton<IFluentQueueApi, FluentQueueApi>(); 
            unityContainer.RegisterFactory<ILog>(f => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType));

            //mappers
            unityContainer.RegisterType<IServiceActionMapper, ServiceActionMapper>();

            //handlers
            unityContainer.RegisterType<IMessageHandler, CaseBackupMessageHandler>();

            //queue service
            unityContainer.RegisterType<IQueueService, QueueService>();

            //blaise services
            unityContainer.RegisterType<IFluentBlaiseApi, FluentBlaiseApi>();

            //logging
            unityContainer.RegisterFactory<ILog>(f => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType));

            //services
            unityContainer.RegisterType<IBackupService, BackupService>();

            //main service
            unityContainer.RegisterType<IInitialiseService, InitialiseService>();


#if (DEBUG)
            var credentialKey = ConfigurationManager.AppSettings["GOOGLE_APPLICATION_CREDENTIALS"];

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialKey);
#endif

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
