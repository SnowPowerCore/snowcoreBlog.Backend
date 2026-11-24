namespace snowcoreBlog.Backend.ReadersManagement.RequestPersistence;

internal static class SnapshotSource
{
    public static readonly System.Text.Json.JsonSerializerOptions Options = new(System.Text.Json.JsonSerializerDefaults.Web);
}
