MaybeResults (preferred result / error pattern)

Purpose
- Explain the repo-wide convention of using the `MaybeResults` source-generator-driven pattern for operation results and errors.

Why we use it
- `MaybeResults` gives a compact way to model operations that can return either an operation result (Some&lt;T&gt;) or a generated "none/error" type. It leads to consistent, source-generated types and simple pattern checks in endpoints/steps.

Common patterns
- Handlers / delegates typically return `Task<IMaybe<TOperationResult>>`.
- Steps and service code use `Maybe.Create(...)` to return a concrete operation result instance early.
- Endpoints typically form HTTP responses using the `ToApiResponse` and `ToStatusCode` extension methods on `IMaybe<T>`. When using the StepifiedProcess pattern you can prefer the compact form below which automatically creates the correct `ApiResponse` depending on whether the result is `Some<T>` or a generator-backed `None`:

```csharp
// endpoint-level pattern (preferred when using stepified delegates)
var result = await SomeDelegate(context, ct);
await Send.ResponseAsync(
    result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
    result?.ToStatusCode() ?? (int)System.Net.HttpStatusCode.InternalServerError,
    ct);
```

If you are not using the StepifiedProcess pattern (for example a plain service method that returns a DTO or throws), you will need to construct the `ApiResponse` and status code manually.

Cross-reference
- The repository-wide standardized `ApiResponse` contract and the recommended endpoint pattern are recorded in the FastEndpoints Endpoint Creation skill (`fastendpoints-endpoints/SKILL.md`). See that skill for the required shape and the recommended endpoint snippet for returning `ApiResponse`.

Analysis: `ToApiResponse` extension
- Signature: `public static ApiResponse ToApiResponse<T>(this IMaybe<T> result, JsonSerializerOptions serializerOptions = null) where T : notnull`.
- Behavior summary:
    - If `result is Some<T> success` then:
        - `dataCount` is computed as `success.Value is ICollection e ? e.Count : 1`.
        - `Data` is produced via `success.Value.ToJsonDocument(serializerOptions)` (uses `JsonSerializer.SerializeToDocument`).
        - Returns `new ApiResponse(Data, DataCount, 0)` (business `StatusCode` == 0, `Errors` empty).
    - If `result is INone<T> error` then:
        - `Errors` are flattened from `error.Details` as `"{Code}: {Description}"`; if empty the extension falls back to `error.Message`.
        - Returns `new ApiResponse(default, 0, -1, errors)` (business `StatusCode` == -1).
    - Otherwise returns a fallback `ApiResponse` with `Errors` containing `ApiResponseConstants.UnknownError`.

- Important notes & recommendations:
    - `ToApiResponse` encodes a domain/business `StatusCode` (0/-1). Use `ToStatusCode()` to produce the HTTP status code when sending the response.
    - `ToApiResponse` serializes the payload to a `JsonDocument` using the provided `JsonSerializerOptions` (source-gen options should be passed when available).
    - `DataCount` only detects counts for types implementing `ICollection`. If you return lazy `IEnumerable<T>` choose to materialize/count before returning or expose an `ICollection` to ensure an accurate `DataCount`.
    - Prefer using `result?.ToApiResponse(...)` + `result?.ToStatusCode()` in endpoints for consistent, compact responses; only build `ApiResponse` manually when you need a custom shape or are outside the stepified flow.

Defining error/result types (source-gen)
- Use attributes the generator expects (examples below). These create small, explicit result/error types that work well in pattern matching.

Example error record (ErrorResults/ReaderAccountNotExistsError.cs)
```csharp
using MaybeResults;

[None]
public partial record ReaderAccountNotExistsError;
```

Example returned operation result (models/RefreshReaderJwtPairOperationResult.cs)
```csharp
public sealed record RefreshReaderJwtPairOperationResult
{
    public int HttpStatusCode { get; init; }
    public ApiResponse Response { get; init; } = default!;
}
```

Usage in a step
```csharp
if (record == null)
    return Maybe.Create(new RefreshReaderJwtPairOperationResult
    {
        HttpStatusCode = 404,
        Response = ErrorResponseUtilities.ApiResponseWithErrors(["Not found"], 404)
    });

// Continue to next step
return await next(context, token);
```

Notes for agents
- Prefer generator-backed `None` error records and `Maybe.Create(...)` for early returns instead of throwing exceptions for expected validation/flow failures.
- Keep operation result types simple and serializable by the source-gen serializer.
