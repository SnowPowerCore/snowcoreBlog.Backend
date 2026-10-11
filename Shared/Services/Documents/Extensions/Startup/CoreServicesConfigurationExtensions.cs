using snowcoreBlog.Backend.Documents.Steps.Documents.Upload;

namespace snowcoreBlog.Backend.Documents.Extensions.Startup;

public static class CoreServicesConfigurationExtensions
{
    public static IServiceCollection AddCoreServicesConfiguration(this IServiceCollection services)
    {
        // Content storage (swap the implementation to move documents to blob storage)
        services.AddScoped<Interfaces.Storage.IDocumentContentStore, Services.Storage.FileSystemDocumentContentStore>();

        // Repositories
        services.AddScoped<Interfaces.Repositories.Marten.IDocumentRepository, Repositories.Marten.DocumentRepository>();

        // Upload pipeline steps (order matters; keep in sync with UploadDocumentEndpoint)
        services.AddScoped<ValidateDocumentUploadStep>();
        services.AddScoped<ComputeDocumentChecksumStep>();
        services.AddScoped<ResolveDuplicateDocumentStep>();
        services.AddScoped<PersistDocumentContentStep>();
        services.AddScoped<CreateDocumentEntityStep>();
        services.AddScoped<PublishDocumentUploadedStep>();
        services.AddScoped<BuildUploadDocumentResultStep>();

        return services;
    }
}
