using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.Infrastructure.Processors;

public class PersistHeaderProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var operation = context.OperationDescription.Operation;
        if (operation.Tags.Contains(ApiTagConstants.Persist))
        {
            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = HeaderKeyConstants.PersistHeader,
                Kind = OpenApiParameterKind.Header,
                Schema = new JsonSchema { Type = JsonObjectType.String },
                IsRequired = true,
                Description = "If set, the request will be persisted for safe reprocessing. Provide a non-empty value."
            });
        }

        return true;
    }
}