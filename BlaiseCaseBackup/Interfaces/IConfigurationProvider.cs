namespace BlaiseCaseBackup.Interfaces
{
    public interface IConfigurationProvider
    {
        string TimerIntervalInMinutes { get; }
        string BucketName { get; }

        string VmName { get; }
    }
}