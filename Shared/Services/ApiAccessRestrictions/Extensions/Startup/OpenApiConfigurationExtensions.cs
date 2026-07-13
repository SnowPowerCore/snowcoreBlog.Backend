namespace snowcoreBlog.Backend.ApiAccessRestrictions.Extensions.Startup;

public static class OpenApiConfigurationExtensions
{
    public static WebApplication UseOpenApiConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseOpenApi(c =>
            {
                c.Path = "/openapi/{documentName}.json";
            });
        }

        return app;
    }
}