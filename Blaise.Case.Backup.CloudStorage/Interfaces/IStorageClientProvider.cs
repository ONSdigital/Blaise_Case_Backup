
using Google.Cloud.Storage.V1;

namespace Blaise.Case.Backup.CloudStorage.Interfaces
{
    public interface IStorageClientProvider
    {
        StorageClient GetStorageClient();

        void Dispose();
    }
}
