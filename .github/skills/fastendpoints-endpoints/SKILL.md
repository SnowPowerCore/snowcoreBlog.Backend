FastEndpoints Endpoint Creation

Purpose
- Make FastEndpoints the standard way to create HTTP endpoints in all backend projects.

When to use
- All new HTTP endpoints should use FastEndpoints. Put endpoint types under `Endpoints/**`.

Key rules
- Use `Endpoint<TRequest, TResponse>` base class and implement `Configure()` and `HandleAsync()`.
- Always set `SerializerContext(CoreSerializationContext.Default)` when the project uses source-gen serialization.
- Use `Version(...)`, `Description(...)`, `Produces<>`, `Accepts<>` and `AllowAnonymous()` consistently.
- Return `ApiResponse` (see `snowcoreBlog.Universal/PublicApi/Utilities/Api`) for HTTP bodies and `ErrorResponseUtilities.ApiResponseWithErrors(...)` for validation/problem results.

Minimal example (simple endpoint)
```csharp
using FastEndpoints;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Articles.Endpoints;

public class CreateArticleEndpoint : Endpoint<CreateArticleRequest, ApiResponse>
{
    public override void Configure()
    {
        Post("articles");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Description(b => b
            .WithTags("Articles")
            .Accepts<CreateArticleRequest>()
            .Produces<ApiResponse>((int)System.Net.HttpStatusCode.OK));
    }

    public override async Task HandleAsync(CreateArticleRequest req, CancellationToken ct)
    {
        // Call services/repositories/mappers here.
        // On success produce a ApiResponse (repo pattern often serializes DTO to JsonDocument first).
        var dto = new ArticleDto { /* ... */ };
        var rsp = new ApiResponse(System.Text.Json.JsonSerializer.SerializeToDocument(dto), 1, 200);
        await Send.ResponseAsync(rsp, 200, ct);
    }
}
```

Notes
- For complex flows prefer the StepifiedProcess pattern (see stepified-process-handlers skill) — ask the human whether the flow is big/dynamic before choosing it.
- Place endpoints in a logical subfolder under `Endpoints/` (group by feature or resource).
- Use the project-level `CoreSerializationContext.Default` for source-generated serialization where available.

Standardized response object
- All API endpoints MUST return a standardized `ApiResponse` object in the response body. The repository-wide shape contains the following properties:
    - `StatusCode` (int): business-level result code (0 = success, -1 = error).
    - `DataCount` (int): number of items contained in `Data` (1 for single objects; collection `Count` when `Data` implements `ICollection`).
    - `Data` (JsonDocument?): serialized payload when successful, or `default`/`null` when an error occurred.
    - `Errors` (IReadOnlyCollection<string>): empty on success; populated when an error occurs.

- Success example: `Data` contains the serialized DTO, `DataCount` > 0, `Errors` is empty, `StatusCode` == 0.
- Error example: `Data` is `default`, `DataCount` == 0, `Errors` contains messages, `StatusCode` == -1.

- Recommended endpoint pattern (preferred):

```csharp
var result = await SomeDelegate(context, ct);
await Send.ResponseAsync(
        result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
        result?.ToStatusCode() ?? (int)System.Net.HttpStatusCode.InternalServerError,
        ct);
```
