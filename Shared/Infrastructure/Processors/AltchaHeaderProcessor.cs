using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using snowcoreBlog.Backend.Core.Constants;

namespace snowcoreBlog.Backend.Infrastructure.Processors;

public class AltchaHeaderProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        if (context.Document.Tags.Any(x => x.Name == EndpointTagConstants.RequireCaptchaVerification))
        {
            var altchaParam = new OpenApiParameter()
            {
                Name = EndpointTagConstants.RequireCaptchaVerification,
                Kind = OpenApiParameterKind.Header,
                IsRequired = true,
                Type = JsonObjectType.String,
                Description = "A required base64 captcha solution that has to be sent along the request."
            };

            context.OperationDescription.Operation.Parameters.Add(altchaParam);
        }

        return true;
    }
}