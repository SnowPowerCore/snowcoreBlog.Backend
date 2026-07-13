using Marten;
using Marten.Services;
using snowcoreBlog.Backend.AuthorsManagement.CompiledQueries.Marten;
using snowcoreBlog.Backend.AuthorsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.AuthorsManagement.Repositories.Marten;
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
            Serializer.UseTypeInfoResolver(CoreSerializationContext.Default);
            opts.Serializer(Serializer);
        })
            .UseLightweightSessions()
            .UseNpgsqlDataSource();

        services.AddScoped<IAuthorRepository, AuthorRepository>();

        return services;
    }

    public static IServiceCollection AddCompiledQueriesConfiguration(this IServiceCollection services)
    {
        services.AddMarten(opts =>
        {
            opts.RegisterCompiledQueryType(typeof(AuthorGetByUserIdQuery));
            opts.RegisterCompiledQueryType(typeof(AuthorExistsByUserIdQuery));
        });

        return services;
    }
}