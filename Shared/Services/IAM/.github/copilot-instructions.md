```instructions
This is the Backend IAM microservice (identity worker — no business HTTP endpoints).

What it does:
- MassTransit consumer service for identity lifecycle: temp user creation, attestation/assertion (Fido2), authenticator (TOTP) setup, one-time-code validation, nickname checks, user deletion, admin invite/create validation.
- Marten stores identity documents; ASP.NET Identity stores (`Stores/MartenUserStore`, `Stores/MartenRoleStore`) back the identity layer.

Key wiring (read `Program.cs` and `Extensions/Startup/*`):
- No FastEndpoints business endpoints: `UseApplicationMiddleware` only enables HTTPS; `MapEndpoints` maps service-default health endpoints only.
- Fido2 (`AddFido2Configuration`), Marten (`db-iam-entities`) with compiled queries in `CompiledQueries/Marten/`, MassTransit consumers registered in `MassTransitConfigurationExtensions`.
- Telemetry/service defaults via `TelemetryConfigurationExtensions`.

Patterns:
- Consumers live in `Features/<Area>/` (`Admin/`, `User/`) and delegate to repositories (`Repositories/Marten`) and services (`Services/TotpService`).
- Validation: FluentValidation validators in `Validation/` wired by `AddValidationConfiguration`.
- Cross-service contracts come from `snowcoreBlog.Universal/PublicApi` (DUPLICATED into frontend/console copies — update all three) and `Shared/IAM.Core` contracts.

When editing:
- Keep consumer reply contracts stable — ReadersManagement/AdminsManagement depend on `ValidateAdminExists`, `CreateAdmin`, user create/validate messages.
- Run a repo-wide backend build after changing any contract.
```
