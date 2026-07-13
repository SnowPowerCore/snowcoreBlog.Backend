using Amazon.Runtime;
using Amazon.SimpleEmailV2;
using SendGrid.Extensions.DependencyInjection;

namespace snowcoreBlog.Backend.Email.Extensions.Startup;

public static class EmailServiceConfigurationExtensions
{
    public static IServiceCollection AddEmailServiceConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSendGrid(options => options.ApiKey = configuration["Integrations:SendGrid:ApiKey"]);
        var awsOption = configuration.GetAWSOptions("Integrations:AWS");
        awsOption.Credentials = new BasicAWSCredentials(configuration["Integrations:AWS:AccessKey"], configuration["Integrations:AWS:SecretKey"]);
        services.AddDefaultAWSOptions(awsOption);
        services.AddAWSService<IAmazonSimpleEmailServiceV2>();

        return services;
    }
}