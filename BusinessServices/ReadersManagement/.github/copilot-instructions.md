```instructions
This is the Backend ReadersManagement microservice. Additions here focus on service-specific wiring and conventions so an AI agent can be productive quickly.

What this service does (big-picture):
- Handles reader account lifecycle: request/confirm create-by-email flows, assertion/attestation for authentication, nickname checks, and related reader entities.
- Uses Marten as the document store for ReaderEntity and Altcha challenge entities. MassTransit is used for a consumer (`ReaderAccountTempUserCreatedConsumer`) triggered by account lifecycle events.

Key wiring and libraries to notice (read `Program.cs`):
- FastEndpoints is used for HTTP endpoints and OpenAPI generation (see `Endpoints/**`). Endpoints often use `SerializerContext(CoreSerializationContext.Default)` and the generated discovered types from source-gen.
- Marten is registered and document types are declared with `options.RegisterDocumentType<ReaderEntity>()` and `options.GeneratedCodeMode = TypeLoadMode.Static` (look at `Internal/Generated/DocumentStorage` for generated providers).
- MassTransit: consumer registration (`AddConsumer<ReaderAccountTempUserCreatedConsumer>()`) and RabbitMQ host config stored in configuration (`rabbitmq` connection string).
- Altcha (Ixnas.AltchaNet) is used for captcha/attestation flows and is registered as a singleton service built with a SHA256 key (see `Program.cs`).
- OpenTelemetry, Redis (cache), and Npgsql data source wiring appear in `Program.cs` and integrate with the Aspire host when run under the Aspire host.

Patterns & conventions used in this codebase (examples):
- Stepified processes: many endpoints use the `[StepifiedProcess]` attribute and delegate types (see `Endpoints/ReaderAccounts/RequestCreateByEmailEndpoint.cs`) to compose validation and steps such as `ValidateReaderAccountEmailDomainStep` and `CreateReaderAccountTempUserStep`.
- Error handling: FastEndpoints problem-details customization and `ErrorResponseUtilities.ApiResponseWithErrors` are used instead of throwing raw exceptions.
- Repositories: implement Marten repositories via `BaseMartenRepository<T>` and register interfaces under `Interfaces/Repositories/Marten` (see `ReaderRepository.cs`).
- Generated code: the repo uses source generators (FastEndpoints generator, Marten static code gen, Mapperly). Look in `Internal/Generated` for generated providers and types.

Developer workflows for this service:
- Run locally as part of the Aspire host (recommended) with `dotnet run --project ./snowcoreBlog.Aspire.csproj` so dependent infra (rabbitmq, redis, postgres) are available.
- Or run the service alone for fast iteration: `dotnet run --project ./snowcoreBlog.Backend.BusinessServices.ReadersManagement/snowcoreBlog.Backend.ReadersManagement.csproj`
- Build and tests: `dotnet build` and `dotnet test` per project.

Files and folders to inspect when making changes:
- `Program.cs` — main wiring: Marten, MassTransit, Altcha, FastEndpoints, OpenTelemetry.
- `Endpoints/**` — FastEndpoints endpoints and attributes (`SerializerContext`, `Version`, `EnableAntiforgery`, stepified delegates).
- `Steps/**` — composable steps used by stepified processes.
- `Repositories/Marten/**` and `Interfaces/Repositories/Marten/**` — persistence patterns and DI registrations.
- `Internal/Generated/**` — Marten and document/storage generated code (update source gen when changing documents).
- `appsettings.json` and `appsettings.Development.json` — local config and connection strings keys (`rabbitmq`, `db-snowcore-blog-entities`, cache, Signing Key under `Security:Signing:User:SigningKey`).

Agent-specific guidance when making edits:
- When changing document types (ReaderEntity, Altcha entities), update Marten registrations in `Program.cs` and ensure static code-gen is regenerated. Run a repo build after changes.
- When changing DTOs that cross services, update the `snowcoreBlog.PublicApi` project and run a repo-wide build to catch generator and serialization issues.
- Reuse existing step types rather than inlining validation logic—this repository favors step composition for flows.
- Keep OpenAPI/Swagger settings consistent: endpoints set `SerializerContext(CoreSerializationContext.Default)` and configure API versioning via FastEndpoints (see `Program.cs` SwaggerDocument config).

If something is unclear, ask for:
- The expected RabbitMQ message contract for the `ReaderAccountTempUserCreatedConsumer` if you need to modify it.
- Whether changes to authentication token signing keys should be applied via user-secrets or environment variables in your deployment environment.

Examples to copy from when implementing features:
- Endpoint using stepified process: `Endpoints/ReaderAccounts/RequestCreateByEmailEndpoint.cs`
- Marten repository: `Repositories/Marten/ReaderRepository.cs`
- Program wiring: `Program.cs`

```
