using System.Text.Json;

namespace snowcoreBlog.Backend.Infrastructure.Extensions;

public static class JsonSerializerOptionsExtensions
{
    public static JsonSerializerOptions SetJsonSerializationContext(this JsonSerializerOptions options)
    {
        options.TypeInfoResolverChain.Insert(0, CoreSerializationContext.Default);
        return options;
    }
}