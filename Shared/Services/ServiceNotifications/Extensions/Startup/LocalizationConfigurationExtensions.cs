using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace snowcoreBlog.Backend.ServiceNotifications.Extensions.Startup;

public static class LocalizationConfigurationExtensions
{
    public const string DefaultCulture = "en";

    public static WebApplication UseLocalizationConfiguration(this WebApplication app)
    {
        var supportedCultures = new[]
        {
            new CultureInfo(DefaultCulture),
            new CultureInfo("tr")
        };

        app.UseRequestLocalization(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(DefaultCulture);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.RequestCultureProviders = [
                new AcceptLanguageHeaderRequestCultureProvider()
            ];
        });

        return app;
    }
}