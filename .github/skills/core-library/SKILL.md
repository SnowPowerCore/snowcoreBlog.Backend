.Core library (shared types and contracts)

Purpose
- Explain the purpose of the shared `snowcoreBlog.Backend.Core` class library and what belongs there.

What to put in `snowcoreBlog.Backend.Core`
- Entities: domain entities (e.g., `ReaderEntity`, `ArticleEntity`) used by Marten document mapping.
- Contracts / DTOs: shared contracts used across backend projects (small, stable DTOs used between services).
- ErrorResults: generator-backed error/result records when shared across services.
- Interfaces: shared repository/service interfaces and compiled query interfaces.
- Constants / Options / Resources: application constants, option POCOs, localization resources.

Location
- The project root is `Shared/Core/snowcoreBlog.Backend.Core.csproj` in this workspace and is included by backend solutions. Use that project for types that cross multiple backend services.

Example layout
- `Shared/Core/Entities/Reader/ReaderEntity.cs`
- `Shared/Core/Constants/Api.cs`
- `Shared/Core/Interfaces/Repositories/IRepository.cs`
- `Shared/Core/Contracts/Events/ReaderAccountTempUserCreated.cs`
- `Shared/Core/ErrorResults/ValidationErrorResult.cs`

Design guidelines
- Keep `Core` stable: changes here ripple across many services and require a repo-wide build.
- Only place things in Core when they truly need to be shared. Prefer service-local types for ephemeral or rapidly-evolving shapes.
- Use `snowcoreBlog.PublicApi` for public DTOs consumed by frontends; use Core for backend-to-backend contracts and domain entities.

Notes for agents
- When proposing new shared types ask whether the type will be used by multiple services. If yes, put it in `Shared/Core` and update project references.
- After changing Core, run a repository build to regenerate any source-generated artifacts (Mapperly, Marten codegen, FastEndpoints source-gen).
