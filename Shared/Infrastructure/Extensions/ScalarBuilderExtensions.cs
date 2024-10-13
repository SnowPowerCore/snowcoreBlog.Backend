using System.Text.Json;
using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.Infrastructure.Extensions;

public static class ScalarBuilderExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    static ScalarBuilderExtensions()
    {
        JsonSerializerOptions.SetJsonSerializationContext();
    }
}