using Marten;
using Marten.Services;
using snowcoreBlog.Backend.AuthorsManagement.CompiledQueries.Marten;
using snowcoreBlog.Backend.AuthorsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.AuthorsManagement.Repositories.Marten;
using snowcoreBlog.Backend.Core.Entities.Author;
using snowcoreBlog.Backend.Infrastructure;

namespace snowcoreBlog.Backend.AuthorsManagement.Extensions.Startup;

public static class MartenConfigurationExtensions
{
    private static readonly SystemTextJsonSerializer Serializer = new();

    public static IServiceCollection AddMartenConfiguration(this IServiceCollection services)
    {
        services.AddMarten(opts =>
        {
            opts.Policies.AllDocumentsSoftDeleted();
            // Admin endpoints query AuthorEntity directly (outside compiled
            // queries), so the document type has to be registered explicitly.
            opts.RegisterDocumentType<AuthorEntity>();
            Serializer.UseTypeInfoResolver(CoreSerializationContext.Default);
            opts.Serializer(Serializer);

            // Compiled queries MUST be registered on the same StoreOptions instance —
            // a second AddMarten call would create a fresh DocumentStore with a default
            // serializer (no TypeInfoResolver) that wins DI and breaks JSON serialization.
            opts.RegisterCompiledQueryType(typeof(AuthorGetByUserIdQuery));
            opts.RegisterCompiledQueryType(typeof(AuthorExistsByUserIdQuery));
            opts.RegisterCompiledQueryType(typeof(AuthorExistsByDisplayNameQuery));
        })
            .UseLightweightSessions()
            .UseNpgsqlDataSource();

        services.AddScoped<IAuthorRepository, AuthorRepository>();

        return services;
    }
}