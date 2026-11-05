using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.Infrastructure.Processors;

/// <summary>
/// OpenAPI: marks <see cref="HeaderKeyConstants.IdempotencyKey"/> as a required header on
/// persist-tagged operations and documents the informative duplicate-status response.
/// </summary>
public class PersistHeaderProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var operation = context.OperationDescription.Operation;
        if (!operation.Tags.Contains(ApiTagConstants.Persist))
        {
            return true;
        }

        operation.Parameters.Add(new OpenApiParameter()
        {
            Name = HeaderKeyConstants.IdempotencyKey,
            Kind = OpenApiParameterKind.Header,
            IsRequired = true,
            Description = "Client-supplied idempotency key; required on persist-tagged endpoints. Retrying with the same key returns an informative status instead of re-executing.",
            Schema = new JsonSchema { Type = JsonObjectType.String }
        });

        return true;
    }
}
