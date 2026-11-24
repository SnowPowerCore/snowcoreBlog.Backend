using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace snowcoreBlog.Backend.Infrastructure.Extensions;

public static class JsonSerializerOptionsExtensions
{
    /// <summary>
    /// Chains the shared Infrastructure serialization context (and any caller-supplied
    /// service-level resolvers, inserted first) into the options' resolver chain.
    /// </summary>
    public static JsonSerializerOptions SetJsonSerializationContext(this JsonSerializerOptions options, params IJsonTypeInfoResolver[] serviceResolvers)
    {
        foreach (var resolver in serviceResolvers)
        {
            options.TypeInfoResolverChain.Insert(0, resolver);
        }

        options.TypeInfoResolverChain.Insert(0, InfrastructureSerializationContext.Default);
        return options;
    }
}
