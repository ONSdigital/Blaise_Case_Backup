using System;
using System.Configuration;
using Blaise.Case.Backup.CloudStorage;
using Blaise.Case.Backup.CloudStorage.Interfaces;
using Blaise.Case.Backup.Core;
using Blaise.Case.Backup.Core.Configuration;
using Blaise.Case.Backup.Core.Interfaces;
using Blaise.Case.Backup.Data;
using Blaise.Case.Backup.Data.Interfaces;
using Blaise.Case.Backup.MessageBroker;
using Blaise.Case.Backup.MessageBroker.Interfaces;
using Blaise.Case.Backup.MessageBroker.Mappers;
using Blaise.Case.Backup.WindowsService.Interfaces;
using Blaise.Nuget.Api;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.PubSub.Api;
using Blaise.Nuget.PubSub.Contracts.Interfaces;
using log4net;
using Unity;

namespace Blaise.Case.Backup.WindowsService.Ioc
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
            _unityContainer.RegisterType<IMessageModelMapper, MessageModelMapper>();

            //handlers
            _unityContainer.RegisterType<IMessageHandler, MessageHandler>();

            //queue service
            _unityContainer.RegisterType<IMessageBrokerService, MessageBrokerService>();

            //blaise services
            _unityContainer.RegisterType<IBlaiseApi, BlaiseApi>();
            _unityContainer.RegisterType<IBlaiseApiService, BlaiseApiService>();

            //logging
            _unityContainer.RegisterFactory<ILog>(f => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType));

            //storage provider
            _unityContainer.RegisterType<IStorageClientProvider, StorageClientProvider>();

            //services
            _unityContainer.RegisterType<IBackupService, BackupService>();
            _unityContainer.RegisterType<IStorageService, StorageService>();

            //main service
            _unityContainer.RegisterType<IInitialiseWindowsService, InitialiseWindowsService>();

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
