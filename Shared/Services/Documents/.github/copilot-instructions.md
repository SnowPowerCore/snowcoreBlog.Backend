```instructions
This is the Backend Documents microservice — the shared content storing / documents uploading service. It is intentionally generic: domain-specific document services (e.g. a future legal-documents management microservice) are expected to reuse this structure, its contracts and its processing pipeline.

What this service does (big-picture):
- Stores document metadata in Marten/Postgres (database `db-documents-entities`) and binary content in a content-addressable store (`IDocumentContentStore`, default file-system implementation under `Documents:Storage:RootPath`, deduplicated by SHA-256 checksum).
- The upload flow is a StepifiedProcess (see `Endpoints/Documents/UploadDocumentEndpoint.cs`): validate → checksum → dedup resolve → persist content → register entity → publish `DocumentUploaded` → build result.
- Publishes `DocumentUploaded` (from `snowcoreBlog.Backend.Core.Contracts`) on RabbitMQ for other services (thumbnailing, indexing, moderation...).

Key wiring (read `Program.cs`):
- Npgsql data source `db-documents-entities`, Marten with `DocumentEntity` + soft-delete policy + `CoreSerializationContext.Default` serializer.
- MassTransit/RabbitMQ (publish-only for now; no consumers yet).
- FastEndpoints with JWT bearer (signing key `Security:Signing:User:SigningKey`), Scalar OpenAPI in Development (`/openapi/v1.json`).
- Options bound from the `Documents` section (`Options/DocumentsOptions.cs`): `PublicBaseUrl`, `Storage:RootPath`, `UploadLimits:MaxSizeBytes` + `UploadLimits:AllowedContentTypes`.

StepifiedProcess conventions here:
- Delegate: `Delegates/UploadDocumentDelegate.cs` (`Task<IMaybe<DocumentDto>>`).
- Context: `Context/UploadDocumentContext.cs` — request part immutable, step outputs (`Checksum`, `ContentKey`, `IsDuplicate`, `Document`) mutable so steps can be re-ordered/extended.
- Steps live in `Steps/Documents/Upload/` and are DI-registered in `Extensions/Startup/CoreServicesConfigurationExtensions.cs`; keep that list in sync with the endpoint's `[StepifiedProcess(Steps = [...])]` array.
- Errors: `[None]` record `ErrorResults/DocumentUploadError.cs` → `DocumentUploadError<DocumentDto>.Create("message")`.

Storage extension points (important for derived services):
- Swap `IDocumentContentStore` (registered in `CoreServicesConfigurationExtensions.cs`) to move content to blob storage — pipeline steps only ever see content keys, never file paths.
- Deduplication is global-by-checksum via `ResolveDuplicateDocumentStep`; scope it per-owner there if a domain needs it.
- Processing is currently synchronous (documents are born `DocumentStatus.Processed`); async domain pipelines can set `Processing`/`Rejected` in their own steps and drive status via the entity.

Endpoints (`Endpoints/Documents/**`, gateway prefix `/api/documents`, service routes are relative):
- `POST upload` — multipart; form fields: `file` (required), `category`, `ownerId`; every extra field becomes document metadata.
- `GET {id:guid}` — metadata; `GET download/{id:guid}` — binary stream; `GET category/{category}` — list; `DELETE {id:guid}` — soft delete (blob retained, content may be shared).
- `GET ping` — healthcheck.

Cross-service touchpoints when changing DTOs/events:
- Entities/enums: `snowcoreBlog.Backend/Shared/Core/Entities/Documents/**`; event contract: `Shared/Core/Contracts/DocumentUploaded.cs`.
- DTOs: `snowcoreBlog.PublicApi` (`DocumentDto`, `DocumentStatusDto`), tag `ApiTagConstants.Documents`; entity↔DTO mapping via Mapperly (`Extensions/DocumentMapper.cs`).
- Register new serializable types in `snowcoreBlog.Backend.Infrastructure/Context/CoreSerializationContext.cs` and keep the 3 PublicApi copies (Backend/Frontend/Console) in sync when a consumer needs them.
- Gateway routes for this service live in the Aspire host `appsettings.Development.json` (`/api/documents/**`).

Run/build:
- Aspire host (recommended): `dotnet run --project ./snowcoreBlog.Aspire.csproj`.
- Standalone: `dotnet run --project ./snowcoreBlog.Backend/Shared/Services/Documents/snowcoreBlog.Backend.Documents.csproj`.
- Tests: `dotnet test ./snowcoreBlog.Backend/Shared/Services/Documents.Tests/snowcoreBlog.Backend.Documents.Tests.csproj`.

Known simplifications (decided, revisit when needed):
- Endpoints are `AllowAnonymous` for now; auth policies (e.g. `regularReader` + owner checks) are expected when a real consumer is wired through the gateway.
- Delete keeps the blob (content-addressable storage may be shared); blob GC is a separate maintenance task.
- No Redis/cache yet — add only when a cached step (like ServiceNotifications' `GetActiveNotificationsCachedStep`) becomes necessary.
```
