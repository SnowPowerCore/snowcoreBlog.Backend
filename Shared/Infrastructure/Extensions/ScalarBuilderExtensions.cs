using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Scalar.AspNetCore;

namespace snowcoreBlog.Backend.Infrastructure.Extensions;

public static class ScalarBuilderExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    static ScalarBuilderExtensions()
    {
        JsonSerializerOptions.TypeInfoResolverChain.Add(CoreSerializationContext.Default);
    }

    public static IEndpointConventionBuilder MapScalarApiReference(this IEndpointRouteBuilder endpoints,
        Action<ScalarOptions> configureOptions)
    {
        var options = new ScalarOptions();
        configureOptions(options);

        var configurationJson = JsonSerializer.Serialize(options, JsonSerializerOptions);

        return endpoints.MapGet(options.EndpointPathPrefix + "{documentName}", (string documentName) =>
        {
            var title = options.Title ?? $"Scalar API Reference -- {documentName}";
            return Microsoft.AspNetCore.Http.Results.Content(
                $$"""
                    <!doctype html>
                    <html>
                    <head>
                        <title>{{title}}</title>
                        <meta charset="utf-8" />
                        <meta name="viewport" content="width=device-width, initial-scale=1" />
                    </head>
                    <body>
                        <script id="api-reference" data-url="{{documentName}}"></script>
                        <script>
                        var configuration = {
                            {{configurationJson}}
                        }
                    
                        document.getElementById('api-reference').dataset.configuration =
                            JSON.stringify(configuration)
                        </script>
                        <script src="https://cdn.jsdelivr.net/npm/@scalar/api-reference"></script>
                    </body>
                    </html>
                    """, "text/html");
        })
        .ExcludeFromDescription();
    }
}