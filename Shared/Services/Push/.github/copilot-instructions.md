```instructions
This is the Backend Push microservice (messaging worker — no business HTTP endpoints).

What it does:
- Consumes push-notification commands from RabbitMQ and sends them to ntfy (`Features/Ntfy/SendPushUsingNtfyConsumer`) via the NtfyCator library.

Key wiring (read `Program.cs` and `Extensions/Startup/*`):
- MassTransit consumers in `MassTransitConfigurationExtensions`; RabbitMQ `rabbitmq` connection string.
- ntfy target/auth configured in `NtfyConfigurationExtensions` (`AddNtfyConfiguration`).
- Service defaults via `TelemetryConfigurationExtensions` in Program.

Patterns:
- One consumer per feature under `Features/<Provider>/`; validation with FluentValidation in `Validation/` (see `GenericPushValidator`).
- Keep the consumer thin — transport concerns belong in `Extensions/NtfyCatorExtensions.cs`.

When editing:
- Push message contracts live in `snowcoreBlog.Universal/PublicApi` (duplicated into frontend/console copies — update all three).
```
