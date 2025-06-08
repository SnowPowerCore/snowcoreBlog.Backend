# Stripe Microservice

This microservice provides Stripe API integration and message-based processing using MassTransit. It is designed to be consistent with other microservices in this repository.

## Features
- MassTransit consumers for Stripe-related events and commands
- Common Stripe API interactions (e.g., create customer, handle webhook, process payment)
- .NET 8 Web API

## Getting Started
1. Configure Stripe API keys in `appsettings.json`.
2. Build and run the service:
   ```pwsh
   dotnet build
   dotnet run
   ```
3. Integrate with your message broker (RabbitMQ) as in other services.

## Development
- Follow patterns from other microservices for consumers and configuration.
- Add new MassTransit consumers in the `Consumers` folder.
- Add Stripe API logic in the `Services` folder.

---
This README will be updated as the service is developed.
