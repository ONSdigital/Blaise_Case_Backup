﻿using Blaise.Case.Backup.Interfaces;
using Google.Cloud.Storage.V1;

namespace Blaise.Case.Backup.Providers
{
    public class StorageClientProvider : IStorageClientProvider
    {
        private StorageClient _storageClient;

        public StorageClient GetStorageClient()
        {
            var client = _storageClient;

            if (client != null)
            {
                return client;
            }

            return (_storageClient = StorageClient.Create());
        }
        public void Dispose()
        {
            _storageClient?.Dispose();
            _storageClient = null;
        }
    }
}
