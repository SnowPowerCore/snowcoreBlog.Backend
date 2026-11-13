```instructions
This is the AspireYarpGateway microservice — the YARP reverse proxy front door for all backend services.

What it does:
- Routes proxied requests to backend microservices via Aspire service discovery, applies security middleware (JWT validation from `snowcoreBlog.Backend.Gateway.Middleware`), and reacts to API access restriction changes (`Features/ApiAccessRestrictionsChangedConsumer` refreshes the in-process compiled snapshot from ApiAccessRestrictions via its GatewayIntegration components).

Key wiring (read `Extensions/WebApplicationBuilderExtensions.cs` and `Extensions/YarpResourceExtensions.cs`):
- YARP config loaded from the Aspire host configuration (`ReverseProxy` section) — routes are NOT declared in this project.
- Cross-service dependency: `snowcoreBlog.Backend.ApiAccessRestrictions` GatewayIntegration types (snapshot store/refresher/recall store) and the `AddApiAccessRestrictionsGatewayIntegration` extension in `ApiAccessRestrictions/Extensions/ServiceCollectionExtensions.cs`. If you move/rename those, update usings here AND in `Shared/Gateway.Middleware`.
- TickerQ dashboard + Redis; OTel via `ConnectBackendServices`.

Patterns:
- Feature consumers under `Features/`; validators in `Validation/`; gateway constants in `Constants/`.
- JWT minting helper for tests/local flows: `Utilities/JwtTokenCreator.cs` (mirrored by AdminsManagement token issuer).

When editing:
- Route changes belong to the Aspire host's `ReverseProxy` configuration, not here.
- Any change to restriction snapshot evaluation must keep `Gateway.Middleware` + ApiAccessRestrictions tests green (run the backend test suite).
```
