```instructions
This is the Backend AuthorsManagement microservice.

What it does:
- "Become author" flow: creates an AuthorEntity for an existing reader (claim-gated), plus admin-side author search/delete. Exposes author display-name availability as an ActualLab.Fusion compute service over RPC (`FusionConfigurationExtensions`, `/fusion/ws`).
- Consumer: `Features/ReturnClaimsIfUserAuthorConsumer` (MassTransit).

Key wiring (read `Program.cs` and `Extensions/Startup/*`):
- FastEndpoints + Fusion (ActualLab.Fusion server) — see `FusionConfigurationExtensions`.
- Marten (`db-snowcore-blog-entities`), Redis, MassTransit.
- Telemetry/service defaults via `TelemetryConfigurationExtensions`.

Patterns (gold standard = ReadersManagement):
- Stepified endpoint: `Steps/AuthorAccounts/Create/CreateAuthorEntityForExistingUserStep`.
- Errors as `[None]` partial records in `ErrorResults/`.
- Marten compiled queries in `CompiledQueries/Marten/` (perf-critical existence checks) — keep using them instead of inline LINQ for hot paths.
- Repositories: `Repositories/Marten/AuthorRepository` + interface in `Interfaces/Repositories/Marten`.

When editing:
- Fusion compute services must stay compatible with the frontend client replica (`snowcoreBlog.Frontend` mirrors the contract).
- Changing author DTOs? Update `snowcoreBlog.Universal/PublicApi` and build repo-wide.
```
