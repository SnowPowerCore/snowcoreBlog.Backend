using System.Text.Json;

namespace snowcoreBlog.Backend.Infrastructure;

public static class FastEndpointsConfigExtensions
{
    public static JsonSerializerOptions SetJsonSerializationContext(this JsonSerializerOptions options)
    {
        options.TypeInfoResolverChain.Insert(0, CoreSerializationContext.Default);
        return options;
    }
}