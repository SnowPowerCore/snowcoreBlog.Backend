This microservice provides regional and IP-based request restriction checks and storage.

Guidance:
- Follows patterns used in ReadersManagement for Marten, MassTransit and FastEndpoints.
- Add new document types under Entities and register in Program.cs.
- Integrate with Aspire YARP gateway by adding middleware call in the gateway resource pipeline (see AspireYarpGateway Extensions).

When adding DTOs exposed to other services, prefer using PublicApi project.
