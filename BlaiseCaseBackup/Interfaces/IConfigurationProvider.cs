namespace BlaiseCaseBackup.Interfaces
{
    public interface IConfigurationProvider
    {
        string BucketName { get; }

        string VmName { get; }

        string ProjectId { get; }

        string SubscriptionTopicId { get; }

        string SubscriptionId { get; }

        string DeadletterTopicId { get; }

        string LocalBackupFolder { get; }

        string SettingsFolder { get; }
    }
}