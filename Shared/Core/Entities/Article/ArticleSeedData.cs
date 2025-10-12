namespace snowcoreBlog.Backend.Core.Entities.Article;

public static class ArticleSeedData
{
    // 10 articles and multiple snapshots per article for seeding / tests
    public static readonly ArticleEntity[] Articles = new[]
    {
        new ArticleEntity
        {
            Id = Guid.Parse("a1111111-0000-4000-8000-000000000001"),
            Title = ".NET 9 Performance Tuning: Practical Tips",
            Slug = "dotnet-9-performance-tuning",
            LatestSnapshotId = Guid.Parse("f1111111-0000-4000-8000-000000000003"),
            PublishedAt = DateTime.Parse("2025-10-05T12:00:00Z").ToUniversalTime(),
            AuthorUserIds = new[] { Guid.Parse("11111111-1111-4111-8111-111111111111") },
            Tags = new[] { "dotnet", "performance", "profiling" },
            CoverImageUrl = null
        },

        new ArticleEntity
        {
            Id = Guid.Parse("a2222222-0000-4000-8000-000000000002"),
            Title = "Minimal APIs in ASP.NET Core: When and How to Use Them",
            Slug = "minimal-apis-aspnet-core",
            LatestSnapshotId = Guid.Parse("f2222222-0000-4000-8000-000000000005"),
            PublishedAt = DateTime.Parse("2025-09-22T10:00:00Z").ToUniversalTime(),
            AuthorUserIds = new[] { Guid.Parse("22222222-2222-4222-8222-222222222222") },
            Tags = new[] { "aspnet-core", "minimal-apis", "web" },
            CoverImageUrl = null
        },

        new ArticleEntity
        {
            Id = Guid.Parse("a3333333-0000-4000-8000-000000000003"),
            Title = "C# Source Generators: A Practical Guide",
            Slug = "csharp-source-generators-guide",
            LatestSnapshotId = Guid.Parse("f3333333-0000-4000-8000-000000000007"),
            PublishedAt = DateTime.Parse("2025-08-18T13:10:00Z").ToUniversalTime(),
            AuthorUserIds = new[] { Guid.Parse("11111111-1111-4111-8111-111111111111") },
            Tags = new[] { "csharp", "source-generators", "roslyn" },
            CoverImageUrl = null
        },

        new ArticleEntity
        {
            Id = Guid.Parse("a4444444-0000-4000-8000-000000000004"),
            Title = "EF Core 8 Migrations: Strategy for Teams",
            Slug = "ef-core-8-migrations-strategy",
            LatestSnapshotId = Guid.Parse("f4444444-0000-4000-8000-000000000009"),
            PublishedAt = DateTime.Parse("2025-07-12T14:00:00Z").ToUniversalTime(),
            AuthorUserIds = new[] { Guid.Parse("33333333-3333-4333-8333-333333333333") },
            Tags = new[] { "ef-core", "migrations", "database" },
            CoverImageUrl = null
        },

        new ArticleEntity
        {
            Id = Guid.Parse("a5555555-0000-4000-8000-000000000005"),
            Title = "Optimizing Blazor WebAssembly Apps for Production",
            Slug = "blazor-wasm-optimization",
            LatestSnapshotId = Guid.Parse("f5555555-0000-4000-8000-000000000010"),
            PublishedAt = DateTime.Parse("2025-06-05T09:00:00Z").ToUniversalTime(),
            AuthorUserIds = new[] { Guid.Parse("22222222-2222-4222-8222-222222222222") },
            Tags = new[] { "blazor", "wasm", "optimization" },
            CoverImageUrl = null
        },

        new ArticleEntity
        {
            Id = Guid.Parse("a6666666-0000-4000-8000-000000000006"),
            Title = "Microservices with MassTransit and RabbitMQ in .NET",
            Slug = "masstransit-rabbitmq-dotnet",
            LatestSnapshotId = Guid.Parse("f6666666-0000-4000-8000-000000000012"),
            PublishedAt = DateTime.Parse("2025-05-20T16:30:00Z").ToUniversalTime(),
            AuthorUserIds = new[] { Guid.Parse("11111111-1111-4111-8111-111111111111") },
            Tags = new[] { "masstransit", "rabbitmq", "microservices" },
            CoverImageUrl = null
        },

        new ArticleEntity
        {
            Id = Guid.Parse("a7777777-0000-4000-8000-000000000007"),
            Title = "Observability in .NET with OpenTelemetry",
            Slug = "opentelemetry-dotnet-observability",
            LatestSnapshotId = Guid.Parse("f7777777-0000-4000-8000-000000000014"),
            PublishedAt = DateTime.Parse("2025-04-22T10:10:00Z").ToUniversalTime(),
            AuthorUserIds = new[] { Guid.Parse("33333333-3333-4333-8333-333333333333") },
            Tags = new[] { "observability", "opentelemetry", "tracing" },
            CoverImageUrl = null
        },

        new ArticleEntity
        {
            Id = Guid.Parse("a8888888-0000-4000-8000-000000000008"),
            Title = "End-to-End Testing .NET Apps with Playwright and WebApplicationFactory",
            Slug = "playwright-webapplicationfactory-testing",
            LatestSnapshotId = Guid.Parse("f8888888-0000-4000-8000-000000000015"),
            PublishedAt = DateTime.Parse("2025-03-10T10:00:00Z").ToUniversalTime(),
            AuthorUserIds = new[] { Guid.Parse("22222222-2222-4222-8222-222222222222") },
            Tags = new[] { "testing", "playwright", "integration" },
            CoverImageUrl = null
        },

        new ArticleEntity
        {
            Id = Guid.Parse("a9999999-0000-4000-8000-000000000009"),
            Title = "Marten as a Postgres-backed Document Store in .NET",
            Slug = "marten-postgresql-document-store",
            LatestSnapshotId = Guid.Parse("f9999999-0000-4000-8000-000000000017"),
            PublishedAt = DateTime.Parse("2025-02-08T14:20:00Z").ToUniversalTime(),
            AuthorUserIds = new[] { Guid.Parse("11111111-1111-4111-8111-111111111111") },
            Tags = new[] { "marten", "postgres", "event-sourcing" },
            CoverImageUrl = null
        },

        new ArticleEntity
        {
            Id = Guid.Parse("aaaaaaaa-0000-4000-8000-00000000000a"),
            Title = "CI/CD for .NET with GitHub Actions",
            Slug = "github-actions-dotnet-ci-cd",
            LatestSnapshotId = Guid.Parse("faaaaaaa-0000-4000-8000-000000000019"),
            PublishedAt = DateTime.Parse("2025-01-15T09:30:00Z").ToUniversalTime(),
            AuthorUserIds = new[] { Guid.Parse("33333333-3333-4333-8333-333333333333") },
            Tags = new[] { "ci-cd", "github-actions", "devops" },
            CoverImageUrl = null
        }
    };

    public static readonly ArticleSnapshotEntity[] Snapshots = new[]
    {
        // Article 1 snapshots
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("f1111111-0000-4000-8000-000000000001"),
            ArticleId = Guid.Parse("a1111111-0000-4000-8000-000000000001"),
            Markdown = "# .NET 9 Performance Tuning - v1\n\nIntroduction and baseline profiling steps...",
            ModifiedAt = DateTime.Parse("2025-10-01T09:05:00Z").ToUniversalTime()
        },
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("f1111111-0000-4000-8000-000000000002"),
            ArticleId = Guid.Parse("a1111111-0000-4000-8000-000000000001"),
            Markdown = "# .NET 9 Performance Tuning - v2\n\nAdded GC and flamegraph notes...",
            ModifiedAt = DateTime.Parse("2025-10-03T15:30:00Z").ToUniversalTime()
        },
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("f1111111-0000-4000-8000-000000000003"),
            ArticleId = Guid.Parse("a1111111-0000-4000-8000-000000000001"),
            Markdown = "# .NET 9 Performance Tuning - v3\n\nFinalized recommendations and benchmarks...",
            ModifiedAt = DateTime.Parse("2025-10-05T11:50:00Z").ToUniversalTime()
        },

        // Article 2 snapshots
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("f2222222-0000-4000-8000-000000000004"),
            ArticleId = Guid.Parse("a2222222-0000-4000-8000-000000000002"),
            Markdown = "# Minimal APIs - v1\n\nWhy minimal APIs were introduced...",
            ModifiedAt = DateTime.Parse("2025-09-20T08:45:00Z").ToUniversalTime()
        },
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("f2222222-0000-4000-8000-000000000005"),
            ArticleId = Guid.Parse("a2222222-0000-4000-8000-000000000002"),
            Markdown = "# Minimal APIs - v2\n\nAdded DI examples and endpoint filters...",
            ModifiedAt = DateTime.Parse("2025-09-22T09:50:00Z").ToUniversalTime()
        },

        // Article 3
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("f3333333-0000-4000-8000-000000000006"),
            ArticleId = Guid.Parse("a3333333-0000-4000-8000-000000000003"),
            Markdown = "# Source Generators - v1\n\nIntro to analyzers and generators...",
            ModifiedAt = DateTime.Parse("2025-08-15T07:20:00Z").ToUniversalTime()
        },
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("f3333333-0000-4000-8000-000000000007"),
            ArticleId = Guid.Parse("a3333333-0000-4000-8000-000000000003"),
            Markdown = "# Source Generators - v2\n\nAdded test strategies with Xunit...",
            ModifiedAt = DateTime.Parse("2025-08-18T12:55:00Z").ToUniversalTime()
        },

        // Article 4
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("f4444444-0000-4000-8000-000000000008"),
            ArticleId = Guid.Parse("a4444444-0000-4000-8000-000000000004"),
            Markdown = "# EF Core 8 Migrations - v1\n\nOverview of code-first migrations...",
            ModifiedAt = DateTime.Parse("2025-07-10T10:05:00Z").ToUniversalTime()
        },
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("f4444444-0000-4000-8000-000000000009"),
            ArticleId = Guid.Parse("a4444444-0000-4000-8000-000000000004"),
            Markdown = "# EF Core 8 Migrations - v2\n\nAdded CI pipeline examples...",
            ModifiedAt = DateTime.Parse("2025-07-12T13:45:00Z").ToUniversalTime()
        },

        // Article 5
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("f5555555-0000-4000-8000-000000000010"),
            ArticleId = Guid.Parse("a5555555-0000-4000-8000-000000000005"),
            Markdown = "# Blazor WASM Optimization - v1\n\nReducing payloads with linker and trimming...",
            ModifiedAt = DateTime.Parse("2025-06-01T11:25:00Z").ToUniversalTime()
        },

        // Article 6
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("f6666666-0000-4000-8000-000000000011"),
            ArticleId = Guid.Parse("a6666666-0000-4000-8000-000000000006"),
            Markdown = "# MassTransit & RabbitMQ - v1\n\nMessage contracts and retry policies...",
            ModifiedAt = DateTime.Parse("2025-05-12T13:05:00Z").ToUniversalTime()
        },
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("f6666666-0000-4000-8000-000000000012"),
            ArticleId = Guid.Parse("a6666666-0000-4000-8000-000000000006"),
            Markdown = "# MassTransit & RabbitMQ - v2\n\nAdded sample topology...",
            ModifiedAt = DateTime.Parse("2025-05-20T16:10:00Z").ToUniversalTime()
        },

        // Article 7
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("f7777777-0000-4000-8000-000000000013"),
            ArticleId = Guid.Parse("a7777777-0000-4000-8000-000000000007"),
            Markdown = "# OpenTelemetry for .NET - v1\n\nSetting up exporters and sampling...",
            ModifiedAt = DateTime.Parse("2025-04-18T09:50:00Z").ToUniversalTime()
        },
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("f7777777-0000-4000-8000-000000000014"),
            ArticleId = Guid.Parse("a7777777-0000-4000-8000-000000000007"),
            Markdown = "# OpenTelemetry for .NET - v2\n\nExamples for Jaeger and Prometheus...",
            ModifiedAt = DateTime.Parse("2025-04-22T09:55:00Z").ToUniversalTime()
        },

        // Article 8
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("f8888888-0000-4000-8000-000000000015"),
            ArticleId = Guid.Parse("a8888888-0000-4000-8000-000000000008"),
            Markdown = "# Playwright & WebApplicationFactory - v1\n\nIntegration testing setup...",
            ModifiedAt = DateTime.Parse("2025-03-03T08:05:00Z").ToUniversalTime()
        },

        // Article 9
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("f9999999-0000-4000-8000-000000000016"),
            ArticleId = Guid.Parse("a9999999-0000-4000-8000-000000000009"),
            Markdown = "# Marten - v1\n\nOverview of document storage and event sourcing...",
            ModifiedAt = DateTime.Parse("2025-02-01T12:35:00Z").ToUniversalTime()
        },
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("f9999999-0000-4000-8000-000000000017"),
            ArticleId = Guid.Parse("a9999999-0000-4000-8000-000000000009"),
            Markdown = "# Marten - v2\n\nAdded examples for session usage and projections...",
            ModifiedAt = DateTime.Parse("2025-02-08T14:00:00Z").ToUniversalTime()
        },

        // Article 10
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("faaaaaaa-0000-4000-8000-000000000018"),
            ArticleId = Guid.Parse("aaaaaaaa-0000-4000-8000-00000000000a"),
            Markdown = "# GitHub Actions for .NET - v1\n\nBasic workflow to build and test .NET projects...",
            ModifiedAt = DateTime.Parse("2025-01-05T06:15:00Z").ToUniversalTime()
        },
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("faaaaaaa-0000-4000-8000-000000000019"),
            ArticleId = Guid.Parse("aaaaaaaa-0000-4000-8000-00000000000a"),
            Markdown = "# GitHub Actions for .NET - v2\n\nAdded deployment steps and semantic versioning...",
            ModifiedAt = DateTime.Parse("2025-01-15T09:10:00Z").ToUniversalTime()
        }
    };
}
