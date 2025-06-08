# Inventory Management System (IMS) Microservice

This microservice provides inventory management capabilities, including:
- Client & Vendor management (groups, categories, details, contacts)
- Sales & Purchase (orders, returns, reports)
- Product & Stock (unit measure, product group, products, delivery, receiving, transfers, adjustments, scrapping, stock counts)
- Reporting (transaction, stock, movement)
- Company settings, tax configuration, user management
- Number sequence for systematic tracking

## Tech Stack
- .NET 9 Web API
- FastEndpoints
- MassTransit (RabbitMQ)
- Marten (PostgreSQL)
- FluentValidation
- OpenTelemetry

## Getting Started
1. Restore dependencies: `dotnet restore`
2. Build the project: `dotnet build`
3. Run the project: `dotnet run`

## Conventions
- Follow patterns from other microservices in this solution.
- Use the `.github/copilot-instructions.md` for Copilot guidance.
