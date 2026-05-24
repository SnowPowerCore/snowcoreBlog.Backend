Riok.Mapperly (source-generated mappers)

Purpose
- Explain how the repo uses `Riok.Mapperly` for mapping DTOs/entities and where to place mapper code.

Key rules
- Put mapper partials under `Extensions/` or `Mappers/` grouped by feature.
- Use the `[Mapper]` attribute on a `partial` static class or on a `partial` interface and declare `partial` mapping methods. The source generator will emit implementations at build time.

Example (Extensions/UserExtensions.cs)
```csharp
using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.Core.Entities.Reader;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

[Mapper]
public static partial class UserExtensions
{
    public static partial ReaderDto ToDto(this ReaderEntity entity);
}
```

Advanced
- Use `MapProperty` attribute for property name mismatches and `MapWith` when using custom converters.
- Keep mapper methods small (single-object mapping or small projections) to keep generated code readable and fast.

Notes for agents
- Mapperly is source generated; ensure the project references `Riok.Mapperly` and that methods are declared `partial` and have the `[Mapper]` attribute.
- If you update DTO or entity shapes, build the project to regenerate mapper implementations and fix mapping compilation errors.
