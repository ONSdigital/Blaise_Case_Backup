using System.IO;
using Blaise.Case.Backup.CloudStorage.Interfaces;

namespace Blaise.Case.Backup.CloudStorage
{
    public class StorageService : IStorageService
    {
        private readonly IStorageClientProvider _storageClient;

        public StorageService(IStorageClientProvider storageClient)
        {
            _storageClient = storageClient;
        }

        public void BackupFilesToBucket(string filePath, string bucketName, string folderName)
        {

            foreach (var file in Directory.GetFiles(filePath))
            {
                UploadFileToBucket(file, bucketName, folderName);
            }
        }

        public void UploadFileToBucket(string filePath, string bucketName, string folderName)
        {
            var fileName = Path.GetFileName(filePath);
            var bucket = _storageClient.GetStorageClient();

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var objectName = folderName == null ? fileName : $"{folderName}/{fileName}";
                bucket.UploadObject(bucketName, objectName, null, fileStream);
            }

            _storageClient.Dispose();
        }
    }
}
