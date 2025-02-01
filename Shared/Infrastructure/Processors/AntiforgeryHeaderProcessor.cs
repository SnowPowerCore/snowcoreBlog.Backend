using Microsoft.AspNetCore.Http;
using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.Infrastructure.Processors;

public class AntiforgeryHeaderProcessor : IOperationProcessor
{
    private readonly List<string> _skipMethods = [HttpMethods.Get,
                                                  HttpMethods.Trace,
                                                  HttpMethods.Options,
                                                  HttpMethods.Head];

    public bool Process(OperationProcessorContext context)
    {
        var operationDesc = context.OperationDescription;
        if (!_skipMethods.Contains(operationDesc.Method))
        {
            operationDesc.Operation.Parameters.Add(new OpenApiParameter()
            {
                Name = HeaderKeyConstants.AntiforgeryHeader,
                Kind = OpenApiParameterKind.Header,
                Schema = new JsonSchema { Type = JsonObjectType.String },
                IsRequired = true,
                Description = "A required antiforgery token that has to be sent along the request with implicit cookie as a pair."
            });
        }

        return true;
    }
}