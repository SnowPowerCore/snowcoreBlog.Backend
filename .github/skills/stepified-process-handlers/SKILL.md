StepifiedProcess Handlers (delegate + context + steps)

Purpose
- Use the Stepified process pattern for large, dynamic, multi-step endpoint logic. This breaks big flows into sequential, testable, injectable steps.

When to use
- Use when the business flow has many conditional branches, multiple persistence or integration steps, or when the logic evolves frequently. For trivial CRUD operations prefer a simple endpoint.
- Always ask the human (or user) whether the flow should be stepified before choosing it.

Core pieces
- Delegate: a `delegate Task<IMaybe<TOperationResult>>(TContext ctx, CancellationToken ct = default)` placed under `Delegates/`.
- Context: a context record/class under `Context/` that holds request data and intermediate values. Inherit from `BaseGenericContext`.
- Steps: classes under `Steps/**` that implement `IStep<TDelegate, TContext, IMaybe<TOperationResult>>`.
- Endpoint: a property decorated with `[StepifiedProcess(Steps = [ typeof(StepA), typeof(StepB) ])]` pointing to the delegate.

Example delegate (Delegates/CreateArticleDelegate.cs)
```csharp
using MaybeResults;
using snowcoreBlog.Backend.Articles.Context;

public delegate Task<IMaybe<CreateArticleOperationResult>> CreateArticleDelegate(CreateArticleContext context, CancellationToken token = default);
```

Example context (Context/CreateArticleContext.cs)
```csharp
using MinimalStepifiedSystem.Base;

public sealed class CreateArticleContext(CreateArticleRequest request) : BaseGenericContext
{
    public CreateArticleRequest Request { get; } = request;
    public ArticleEntity? Article { get; set; }
    public CreateArticleOperationResult? Result { get; set; }
}
```

Example step (Steps/ValidateArticleTitleStep.cs)
```csharp
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;

public sealed class ValidateArticleTitleStep() : IStep<CreateArticleDelegate, CreateArticleContext, IMaybe<CreateArticleOperationResult>>
{
    public Task<IMaybe<CreateArticleOperationResult>> InvokeAsync(CreateArticleContext context, CreateArticleDelegate next, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(context.Request.Title))
            return Task.FromResult(Maybe.Create(new CreateArticleOperationResult { HttpStatusCode = 400, Response = ErrorResponseUtilities.ApiResponseWithErrors(["Title is required"], 400) }));

        return next(context, token);
    }
}
```

Endpoint usage example (Endpoints/Articles/CreateArticleEndpoint.cs)
```csharp
using MinimalStepifiedSystem.Attributes;

public class CreateArticleEndpoint : Endpoint<CreateArticleRequest, ApiResponse>
{
    [StepifiedProcess(Steps = [ typeof(ValidateArticleTitleStep), typeof(PersistArticleStep), typeof(PublishArticleEventStep) ])]
    protected CreateArticleDelegate CreateArticle { get; } = default!;

    public override void Configure()
    {
        Post("articles");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
    }

    public override async Task HandleAsync(CreateArticleRequest req, CancellationToken ct)
    {
        var ctx = new CreateArticleContext(req);
        var maybe = await CreateArticle(ctx, ct);
        var op = maybe is Some<CreateArticleOperationResult> s ? s.Value : new CreateArticleOperationResult { HttpStatusCode = 500, Response = ErrorResponseUtilities.ApiResponseWithErrors(["Unknown error"], 500) };
        await Send.ResponseAsync(op.Response, op.HttpStatusCode, ct);
    }
}
```

Registration notes
- Register each step class and repository/service used by steps in `Program.cs` with `builder.Services.AddScoped<ValidateArticleTitleStep>();` and register the repository interface/impl.
- Keep steps small and focused (single responsibility) so they are reusable and easy to test.
