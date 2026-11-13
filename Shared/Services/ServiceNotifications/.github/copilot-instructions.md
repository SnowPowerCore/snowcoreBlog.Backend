```instructions
This is the Backend ServiceNotifications microservice.

What it does:
- CRUD + reads for user-facing notifications: create/update/delete notification entities (Marten), cached active-notifications reads (Redis, `Constants/NotificationCacheKeys`), per-user and by-id queries.

Key wiring (read `Program.cs` and `Extensions/Startup/*`):
- FastEndpoints, Marten (`db-snowcore-blog-entities`), Redis (`cache`), MassTransit.
- Telemetry/service defaults via `TelemetryConfigurationExtensions` (`AddOpenTelemetryConfiguration`).

Patterns (gold standard = ReadersManagement):
- Stepified endpoints with per-operation step folders: `Steps/Notification/{Create,Delete,Get,Update}` (validate-then-mutate pattern per operation).
- Errors as `[None]` partial records in `ErrorResults/` (`NotificationNotFoundError`, `NotificationValidationError`).
- Repositories: `Repositories/Marten/NotificationRepository` + interface in `Interfaces/Repositories/Marten`; registered in `CoreServicesConfigurationExtensions`.
- Redis cache keys live in `Constants/NotificationCacheKeys.cs` — never inline them.

When editing:
- Notification DTOs live in `snowcoreBlog.Universal/PublicApi` (duplicated into frontend copies — update all three) and build repo-wide.
```
