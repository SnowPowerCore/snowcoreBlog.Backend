MassTransit consumers (Features/**)

Purpose
- Document the MassTransit consumer pattern used in backend services and where to find consumers (`Features/**`).

When to use
- Implement a consumer when you need to react to messages/events from other services (RabbitMQ via MassTransit). Place feature consumers under a `Features/**` folder inside service project.

Key patterns
- Consumers implement `IConsumer<TMessage>` and live in `Features/<feature>/` or `Features/**`.
- Register consumers in `Program.cs` via `busConfigurator.AddConsumer<YourConsumer>();` and MassTransit configuration will call `config.ConfigureEndpoints(context)`.

Minimal example (Features/ReaderAccountTempUserCreatedConsumer.cs)
```csharp
using MassTransit;

namespace snowcoreBlog.Backend.ReadersManagement.Features;

public sealed class ReaderAccountTempUserCreatedConsumer : IConsumer<ReaderAccountTempUserCreatedEvent>
{
    readonly IReaderRepository _repo;

    public ReaderAccountTempUserCreatedConsumer(IReaderRepository repo) => _repo = repo;

    public async Task Consume(ConsumeContext<ReaderAccountTempUserCreatedEvent> context)
    {
        var msg = context.Message;
        // call repository/service to react to the event
        await _repo.AddOrUpdateAsync(new ReaderEntity { /* map fields from msg */ });
    }
}
```

Notes
- Keep consumers idempotent: they may be retried and should tolerate duplicates.
- Prefer composing consumers with repository/service abstractions rather than embedding business logic in the consumer directly.
- Tests: add consumer tests under `Features/**/Tests` and use in-memory MassTransit testing helpers when available.
