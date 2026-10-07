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
            CoverImageUrl = "https://picsum.photos/1920/1080"
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
            CoverImageUrl = "https://picsum.photos/1920/1080"
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
            CoverImageUrl = "https://picsum.photos/1920/1080"
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
            CoverImageUrl = "https://picsum.photos/1920/1080"
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
            CoverImageUrl = "https://picsum.photos/1920/1080"
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
            CoverImageUrl = "https://picsum.photos/1920/1080"
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
            CoverImageUrl = "https://picsum.photos/1920/1080"
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
            CoverImageUrl = "https://picsum.photos/1920/1080"
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
            CoverImageUrl = "https://picsum.photos/1920/1080"
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
            CoverImageUrl = "https://picsum.photos/1920/1080"
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
            Markdown = """
                # .NET 9 Performance Tuning - v3

                Final recommendations after three rounds of profiling a mid-size e-commerce API.

                ## Baseline first

                Measure before changing anything. A minimal BenchmarkDotNet harness keeps the noise low:

                ```csharp
                [MemoryDiagnoser]
                public class OrderPipelineBenchmarks
                {
                    [Benchmark(Baseline = true)]
                    public OrderResult CurrentPipeline() => Pipeline.Run(Order.Sample);

                    [Benchmark]
                    public OrderResult OptimizedPipeline() => FastPipeline.Run(Order.Sample);
                }
                ```

                ## What moved the needle

                | Change                          | p95 (ms) | Allocated (MB) |
                | ------------------------------- | -------- | -------------- |
                | Pool `DbContext` instances      | 142      | 18.2           |
                | JSON source generators          | 118      | 9.7            |
                | `FrozenDictionary` for lookups  | 96       | 4.1            |

                ## What did not

                - Micro-optimizing LINQ in cold paths
                - `Span<T>` in request handlers
                - Custom allocators outside hot loops

                > Profile the whole request path. Local wins rarely move p95.

                ## Follow-ups

                - [x] Dashboards for GC pauses and thread-pool queue
                - [x] Alerts on p95 regressions > 10%
                - [ ] Re-run benchmarks on .NET 10 previews
                """,
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
            Markdown = """
                # Minimal APIs in ASP.NET Core - v2

                Second revision: route groups, filters, and where minimal APIs stop being minimal.

                ## Route groups keep it tidy

                ```csharp
                var orders = app.MapGroup("/orders")
                    .RequireAuthorization()
                    .WithTags("Orders");

                orders.MapGet("/{id:guid}", GetOrderById.Handle);
                orders.MapPost("/", CreateOrder.Handle);
                ```

                ## Endpoint filters beat middleware for per-route work

                - Validation
                - Logging scoped to one route
                - Response shaping

                ## When to go back to controllers

                1. Large action surfaces with shared conventions
                2. Heavy model-binding customization
                3. Teams with existing MVC tooling

                > Minimal APIs are not a religion. Pick them per service, not per endpoint.
                """,
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
            Markdown = """
                # C# Source Generators - v2

                Revision two: incremental generators and test strategies.

                ## Incremental, not ISourceGenerator

                `IIncrementalGenerator` pipelines cache per-change and keep IDEs fast:

                ```csharp
                [Generator]
                public sealed class DtoMapperGenerator : IIncrementalGenerator
                {
                    public void Initialize(IncrementalGeneratorInitializationContext context)
                    {
                        var dtos = context.SyntaxProvider
                            .CreateSyntaxProvider(
                                static (node, _) => node is RecordDeclarationSyntax,
                                static (ctx, _) => GetDtoModel(ctx))
                            .Where(static m => m is not null);

                        context.RegisterSourceOutput(dtos, EmitMapper);
                    }
                }
                ```

                ## Test the output, not the generator

                1. Snapshot-test generated files
                2. Compile generated code with `[CSharpCompiler]` harnesses
                3. Assert diagnostics for invalid inputs

                > If your generator runs on every keystroke, the pipeline is wrong.
                """,
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
            Markdown = """
                # EF Core 8 Migrations - v2

                Added the team workflow we settled on after three release trains.

                ## One migration, one PR

                Small migrations review well and roll back safely:

                ```bash
                dotnet ef migrations add AddOrderDiscount --project src/Orders
                dotnet ef migrations script $prev $head -o artifacts/migrate.sql
                ```

                ## Rules that survived contact with production

                - Never edit an applied migration
                - Generate idempotent scripts and store them as artifacts
                - Test migrations against a restored production snapshot

                ```csharp
                migrationBuilder.Sql(
                    "UPDATE orders SET discount = 0 WHERE discount IS NULL;");
                ```

                > The database does not care about your branch strategy. The migration history does.
                """,
            ModifiedAt = DateTime.Parse("2025-07-12T13:45:00Z").ToUniversalTime()
        },

        // Article 5
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("f5555555-0000-4000-8000-000000000010"),
            ArticleId = Guid.Parse("a5555555-0000-4000-8000-000000000005"),
            Markdown = """
                # Optimizing Blazor WebAssembly Apps for Production

                Shipping Blazor WASM to production is mostly about bytes and startup time.
                This post collects the switches that actually matter.

                ## Trim and AOT

                Two settings dominate payload size and runtime speed:

                ```xml
                <PropertyGroup>
                  <PublishTrimmed>true</PublishTrimmed>
                  <RunAOTCompilation>true</RunAOTCompilation>
                  <InvariantGlobalization>true</InvariantGlobalization>
                </PropertyGroup>
                ```

                | Mode            | Download | Time-to-interactive |
                | --------------- | -------- | ------------------- |
                | No trimming     | 12.4 MB  | 9.1 s               |
                | Trimmed         | 5.2 MB   | 4.3 s               |
                | Trimmed + AOT   | 5.8 MB   | 3.4 s               |

                AOT costs ~10% download and repays it on CPU-bound screens.

                ## Keep the linker honest

                Every reflection surface needs a hint, or the linker drops it:

                ```csharp
                [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(OrderDto))]
                [JsonSerializable(typeof(OrderDto))]
                internal partial class AppJsonContext : JsonSerializerContext;
                ```

                ## Lazy-load modules

                - Split rarely used screens into separate Razor class libraries
                - Load their assemblies on first navigation, not at boot
                - Cache loaded assemblies per session

                > Measure with `dotnet publish` output. Dev builds hide linker effects.

                ## Runtime checklist

                - [x] Brotli compression at the CDN
                - [x] Integrity headers for `blazor.boot.json`
                - [ ] HTTP/3 where the host supports it
                """,
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
            Markdown = """
                # Microservices with MassTransit and RabbitMQ - v2

                Second revision with the topology and retry policies we run in production.

                ## Contracts are public API

                Version them, review them, never reuse a command for a second purpose.

                ```csharp
                cfg.ReceiveEndpoint("order-submitted", e =>
                {
                    e.ConfigureConsumer<OrderSubmittedConsumer>(context);
                    e.UseMessageRetry(r => r.Intervals(
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromMinutes(1)));
                });
                ```

                ## Reliability rules

                1. Consumers are idempotent or the transport is lying to you
                2. Dead-letter everything after the retry budget
                3. One exchange per event, fanout per consumer

                | Signal              | Healthy   | Investigate |
                | ------------------- | --------- | ----------- |
                | Consumer lag        | < 1 s     | > 30 s      |
                | Retry rate          | < 0.1%    | > 1%        |

                > Outbox or it did not happen.
                """,
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
            Markdown = """
                # Observability in .NET with OpenTelemetry - v2

                Revision two: exporter setup for Jaeger and Prometheus plus tail-based sampling.

                ## One wiring, many backends

                ```csharp
                builder.Services.AddOpenTelemetry()
                    .ConfigureResource(r => r.AddService("orders-api"))
                    .WithTracing(t => t
                        .AddAspNetCoreInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation()
                        .AddOtlpExporter())
                    .WithMetrics(m => m
                        .AddRuntimeInstrumentation()
                        .AddPrometheusExporter());
                ```

                ## What to actually sample

                - 100% of errors
                - 100% of slow requests (p99 threshold)
                - 1-5% of the rest

                ## Dashboards worth building

                | Panel              | Signal                       |
                | ------------------ | ---------------------------- |
                | p95 per route      | Regression detection         |
                | GC pause time      | Memory pressure              |
                | DB command duration| Query regressions            |

                > Traces answer questions. Metrics only raise them.
                """,
            ModifiedAt = DateTime.Parse("2025-04-22T09:55:00Z").ToUniversalTime()
        },

        // Article 8
        new ArticleSnapshotEntity
        {
            Id = Guid.Parse("f8888888-0000-4000-8000-000000000015"),
            ArticleId = Guid.Parse("a8888888-0000-4000-8000-000000000008"),
            Markdown = """
                # End-to-End Testing .NET Apps with Playwright and WebApplicationFactory

                The full setup: real browser against a real host, in CI, in minutes.

                ## Spin up the app for real

                ```csharp
                public class BlogWebApplicationFactory
                    : WebApplicationFactory<Program>
                {
                    protected override void ConfigureWebHost(IWebHostBuilder builder)
                    {
                        builder.UseSetting("ConnectionStrings:db", _pg.ConnectionString);
                    }
                }
                ```

                ## Drive the browser

                ```csharp
                await page.GotoAsync("/articles/blazor-wasm-optimization");
                await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Optimizing" }))
                    .ToBeVisibleAsync();
                ```

                ## Rules

                1. E2E tests cover user journeys, not edge cases
                2. Unit tests stay fast; E2E tests stay few
                3. One containerized database per run, never shared

                > A flaky E2E suite is worse than none. Fix flakes the same hour they appear.
                """,
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
            Markdown = """
                # Marten as a Postgres-backed Document Store - v2

                Revision two: session usage patterns and projection choices.

                ## Sessions are your unit of work

                ```csharp
                await using var session = store.LightweightSession();
                session.Store(order);
                session.Events.Append(order.Id, new OrderSubmitted(order.Id));
                await session.SaveChangesAsync();
                ```

                ## Query when you can, project when you must

                - `Query<T>()` for document reads
                - Inline projections for dashboards
                - Async projections when the write side is hot

                | Need                    | Tool                     |
                | ----------------------- | ------------------------ |
                | Load by id              | `LoadAsync<T>`           |
                | Filtered lists          | `Query<T>().Where(...)`  |
                | Read model from events  | Projections              |

                > Postgres does the heavy lifting. Marten just stops fighting it.
                """,
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
            Markdown = """
                # CI/CD for .NET with GitHub Actions - v2

                Revision two: deployment jobs and semantic versioning on top of the build workflow.

                ## Build and test

                ```yaml
                jobs:
                  build:
                    runs-on: ubuntu-latest
                    steps:
                      - uses: actions/checkout@v4
                      - uses: actions/setup-dotnet@v4
                        with:
                          dotnet-version: 10.0.x
                      - run: dotnet build -bl:artifacts/build.binlog
                      - run: dotnet test --collect:"XPlat Code Coverage"
                ```

                ## Deploy with gates

                1. Package on green main
                2. Deploy to staging automatically
                3. Promote to production on approval

                | Environment | Trigger         | Approval |
                | ----------- | --------------- | -------- |
                | staging     | push to main    | no       |
                | production  | manual promote  | yes      |

                > Version from git tags, not from a file someone forgets to bump.
                """,
            ModifiedAt = DateTime.Parse("2025-01-15T09:10:00Z").ToUniversalTime()
        }
    };
}