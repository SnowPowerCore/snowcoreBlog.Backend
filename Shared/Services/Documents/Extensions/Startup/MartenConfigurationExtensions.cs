using Marten;
using Marten.Services;
using snowcoreBlog.Backend.Core.Entities.Documents;
using snowcoreBlog.Backend.Documents.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Documents.Repositories.Marten;
using snowcoreBlog.Backend.Infrastructure;

namespace snowcoreBlog.Backend.Documents.Extensions.Startup;

public static class MartenConfigurationExtensions
{
    private static readonly SystemTextJsonSerializer _serializer = new();

    public static IServiceCollection AddMartenConfiguration(this IServiceCollection services)
    {
        services.AddMarten(options =>
        {
            options.RegisterDocumentType<DocumentEntity>();
            _serializer.UseTypeInfoResolver(CoreSerializationContext.Default);
            options.Serializer(_serializer);
            options.Policies.AllDocumentsSoftDeleted();
        })
            .UseLightweightSessions()
            .UseNpgsqlDataSource();

        services.AddScoped<IDocumentRepository, DocumentRepository>();

        return services;
    }
}
