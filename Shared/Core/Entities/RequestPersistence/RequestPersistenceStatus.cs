namespace snowcoreBlog.Backend.Core.Entities.RequestPersistence;

public enum RequestPersistenceStatus
{
    Received = 0,
    Persisted = 1,
    Processing = 2,
    Succeeded = 3,
    Failed = 4,
    RetryNeeded = 5
}