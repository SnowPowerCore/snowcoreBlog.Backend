using System.Text.Json;

namespace snowcoreBlog.Backend.Infrastructure.Extensions;

public static class RabbitMqBusFactoryConfiguratorExtensions
{
    public static void SetJsonSerializationContext(this JsonSerializerOptions options) =>
        options.TypeInfoResolverChain.Insert(0, CoreSerializationContext.Default);
}