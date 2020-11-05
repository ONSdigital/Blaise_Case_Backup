
namespace Blaise.Case.Backup.Interfaces
{
    public interface IBucketService
    {
        void BackupFilesToBucket(string filePath, string bucketName, string folderName);
    }
}
