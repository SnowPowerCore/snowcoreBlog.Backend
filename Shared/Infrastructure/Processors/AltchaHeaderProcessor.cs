using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using snowcoreBlog.Backend.Core.Constants;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.Infrastructure.Processors;

public class AltchaHeaderProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var operation = context.OperationDescription.Operation;
        if (operation.Tags.Any(x => x == EndpointTagConstants.RequireCaptchaVerification))
        {
            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = HeaderKeyConstants.CaptchaHeader,
                Kind = OpenApiParameterKind.Header,
                Schema = new JsonSchema { Type = JsonObjectType.String },
                IsRequired = true,
                Description = "A required base64 captcha solution that has to be sent along the request."
            });
        }

        return true;
    }
}