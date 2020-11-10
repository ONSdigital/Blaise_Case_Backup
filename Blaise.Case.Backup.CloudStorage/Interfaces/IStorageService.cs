
namespace Blaise.Case.Backup.CloudStorage.Interfaces
{
    public interface IStorageService
    {
        void BackupFilesToBucket(string filePath, string bucketName, string folderName);
    }
}
