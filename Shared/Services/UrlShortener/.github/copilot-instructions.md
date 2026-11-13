```instructions
This is the Backend UrlShortener microservice.

What it does:
- Short-code creation and redirect resolution with Redis-backed rate limiting and click recording. Marten stores `UrlMappingEntity` (+ click events).

Key wiring (read `Program.cs` and `Extensions/Startup/*`):
- FastEndpoints, Marten (`db-snowcore-blog-entities`, lightweight sessions), Redis (`cache`).
- Options bound from the `UrlShortener` section (`AddUrlShortenerOptionsConfiguration` in `HostingConfigurationExtensions`): BaseUrl, DefaultMaxClicksPerWindow, DefaultWindowSeconds.

Patterns (gold standard = ReadersManagement):
- Stepified endpoints: `Steps/Create/*` (validate URL → generate code → persist → build result) and `Steps/Resolve/*` (load → check active → rate limit → record click → build result). Steps registered in `CoreServicesConfigurationExtensions`; repository in `AddUrlMappingRepositoriesConfiguration`.
- Repositories: `Repositories/Marten/UrlMappingRepository` (extends `BaseMartenRepository<T>`) + interface in `Interfaces/Repositories/Marten`.
- Operation results in `Models/` carry inline error responses (`ErrorResponseUtilities.ApiResponseWithErrors`) so steps control exact status codes (400/404/429).

When editing:
- Preserve the create/resolve step ordering — the resolve pipeline short-circuits on first error result.
- Rate-limit and click-recording are Redis-backed and best-effort; do not make redirect fail when Redis is down.
```
