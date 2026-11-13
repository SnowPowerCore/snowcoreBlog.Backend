```instructions
This is the Backend AdminsManagement microservice.

What it does:
- Admin account lifecycle: invite-only creation (accept invite → IAM identity + Marten AdminEntity), email login links (single-use AdminLoginTokenEntity), soft-delete with last-active-admin guard, paged admin search, admin session cookie issuance (AdminTokenPairIssuer).
- Admins CANNOT self-register; invites are the only path to a new admin.

Key wiring (read `Program.cs` and `Extensions/Startup/*`):
- FastEndpoints + admin signing key (`Security:Signing:Admin:SigningKey`, issuer "admin").
- Marten (`db-admins-entities`) with soft-delete + FK/index policies (see `MartenConfigurationExtensions`).
- MassTransit to IAM: `IRequestClient<CreateAdmin>`, `IRequestClient<ValidateAdminExists>`.
- Emails are published on the bus via `AdminEmailPublisher` (backend-email consumes them).

Patterns (gold standard = ReadersManagement):
- Endpoints are thin: `[StepifiedProcess]` delegate + context (see `Endpoints/Admins/DeleteAdminAccountEndpoint.cs`).
- Flows return operation results (`Models/*OperationResult`) so steps control exact status codes (404/409 preserved); steps build responses with `ErrorResponseUtilities.ApiResponseWithErrors` and serialize DTOs with `IOptions<JsonOptions>`.
- Repositories: `Repositories/Marten/*` extend `BaseMartenRepository<T>`; interfaces live in `Interfaces/Repositories/Marten`; register in `AddAdminRepositoriesConfiguration`.
- Step + service registration happens in `CoreServicesConfigurationExtensions`.
- Principal/DTO helpers live in `Extensions/AdminPrincipalExtensions.cs` (single source — do not duplicate).

When editing:
- Keep response envelopes/status codes identical when moving logic between steps and endpoints.
- New flows: create Context + Delegate + Models op-result + Steps under `Steps/<Feature>/<Op>/`, register all steps.
- Changing IAM contracts? Update `snowcoreBlog.Universal/PublicApi` + IAM.Core.Contracts and build repo-wide.
```
