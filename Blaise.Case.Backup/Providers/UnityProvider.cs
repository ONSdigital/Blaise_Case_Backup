
using System;
using System.Configuration;
using Blaise.Case.Backup.Interfaces;
using Blaise.Case.Backup.Mappers;
using Blaise.Case.Backup.MessageHandler;
using Blaise.Case.Backup.Services;
using Blaise.Nuget.Api;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.PubSub.Api;
using Blaise.Nuget.PubSub.Contracts.Interfaces;
using log4net;
using Unity;

namespace Blaise.Case.Backup.Providers
{
    public class UnityProvider
    {
        private readonly IUnityContainer _unityContainer;

        public UnityProvider()
        {
            _unityContainer = new UnityContainer();
            _unityContainer.RegisterSingleton<IFluentQueueApi, FluentQueueApi>();
            _unityContainer.RegisterFactory<ILog>(f => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType));

            //mappers
            _unityContainer.RegisterType<IServiceActionMapper, ServiceActionMapper>();

            //handlers
            _unityContainer.RegisterType<IMessageHandler, CaseBackupMessageHandler>();

            //queue service
            _unityContainer.RegisterType<IQueueService, QueueService>();

            //blaise services
            _unityContainer.RegisterType<IBlaiseApi, BlaiseApi>();

            //logging
            _unityContainer.RegisterFactory<ILog>(f => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType));

            //storage provider
            _unityContainer.RegisterType<IStorageClientProvider, StorageClientProvider>();

            //services
            _unityContainer.RegisterType<IBackupService, BackupService>();
            _unityContainer.RegisterType<IBucketService, BucketService>();

            //main service
            _unityContainer.RegisterType<IInitialiseService, InitialiseService>();

#if (DEBUG)
            var credentialKey = ConfigurationManager.AppSettings["GOOGLE_APPLICATION_CREDENTIALS"];

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialKey);
            _unityContainer.RegisterType<IConfigurationProvider, LocalConfigurationProvider>();
#else
            _unityContainer.RegisterType<IConfigurationProvider, ConfigurationProvider>();
#endif

        }

        public T Resolve<T>()
        {
            return _unityContainer.Resolve<T>();
        }
    }
}
