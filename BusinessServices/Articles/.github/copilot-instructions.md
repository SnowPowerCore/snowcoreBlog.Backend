```instructions
This is the Backend Articles microservice.

What it does:
- Article creation (slug generation, author validation, snapshot persistence) and cached reads (by-slug, latest list) backed by Marten (`ArticleEntity`/`ArticleSnapshotEntity`) and Redis caching (TTL-based, `ArticlesCacheKeys`).
- Altcha captcha + antiforgery endpoints shared with the readers flow.

Key wiring (read `Program.cs` and `Extensions/Startup/*`):
- FastEndpoints, Marten (`db-snowcore-blog-article-entities`), Redis (`cache`), Altcha challenge store.
- Telemetry/service defaults via `TelemetryConfigurationExtensions` (`UseServiceDefaultsConfiguration`, `AddOpenTelemetryConfiguration`).

Patterns (gold standard = ReadersManagement):
- Stepified endpoints: `Steps/Articles/Create/*` (create flow), `Steps/Articles/Get/*` (reads). Steps registered in `CoreServicesConfigurationExtensions`.
- Errors as `[None]` partial records in `ErrorResults/` (see `ArticleNotFoundError`).
- Repositories: `Repositories/Marten/ArticleRepository` + `Interfaces/Repositories/Marten/IArticleRepository`.
- Cache keys live in `Constants/ArticlesCacheKeys.cs` — never inline Redis keys.

When editing:
- Keep caching best-effort (try/catch around Redis) — stale entries expire via TTL.
- Changing article DTOs? Update `snowcoreBlog.Universal/PublicApi` and build repo-wide (serialization contexts).
```
