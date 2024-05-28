using Marten;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using snowcoreBlog.Backend.IAM.Entities;
using snowcoreBlog.Backend.IAM.Extensions;
using snowcoreBlog.Backend.IAM.Models;
using snowcoreBlog.Backend.IAM.Options;
using snowcoreBlog.ServiceDefaults.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.Configure<MassTransitHostOptions>(options =>
{
    options.WaitUntilStarted = true;
});

builder.Services.Configure<Argon2PasswordHasherOptions>(options =>
{
    options.Strength = Argon2HashStrength.Moderate;
});

builder.AddServiceDefaults();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource("Marten"))
    .WithMetrics(metrics => metrics.AddMeter("Marten"));
builder.Services.AddNpgsqlDataSource("db-iam-entities");
builder.Services.AddMarten(opts =>
{
    opts.Policies.AllDocumentsSoftDeleted();
})
    .UseNpgsqlDataSource();
builder.Services
    .AddIdentityCore<ApplicationUser>()
    .AddMartenStores<ApplicationUser, IdentityRole>()
    .AddDefaultTokenProviders();
builder.Services
    .AddIdentityCore<ApplicationAdmin>()
    .AddMartenStores<ApplicationAdmin, IdentityRole>()
    .AddDefaultTokenProviders();
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.UsingRabbitMq((context, config) =>
    {
        config.Host(builder.Configuration.GetConnectionString("rabbitmq"));
    });
});
//builder.Services.AddMultipleAuthentications(
//    builder.Configuration["Security:Signing:User:Key"]!,
//    builder.Configuration["Security:Signing:Admin:Key"]!);
//builder.Services.AddAuthorization()
// .AddFastEndpoints(static o =>
// {
//     o.SourceGeneratorDiscoveredTypes.AddRange(DiscoveredTypes.All);
// })
// .AddCors();

// app.UseCookiePolicy(new()
// {
//     MinimumSameSitePolicy = SameSiteMode.Strict,
//     HttpOnly = HttpOnlyPolicy.Always,
//     Secure = CookieSecurePolicy.Always
// })
//.UseMiddleware<CookieJsonWebTokenMiddleware>()
//.UseAuthentication()
//.UseAuthorization();
//.UseFastEndpoints(c =>
//{
//    c.Errors.ResponseBuilder = static (failures, ctx, statusCode) =>
//     {
//         var failuresDict = failures
//             .GroupBy(f => f.PropertyName)
//             .ToDictionary(
//                 keySelector: e => e.Key,
//                 elementSelector: e => e.Select(m => $"{e.Key}: {m.ErrorMessage}").ToArray());

//         return ErrorResponseUtilities.ApiResponseWithErrors(
//             failuresDict.Values.SelectMany(x => x.Select(s => s)).ToList(), statusCode);
//     };
//});

await builder.Build().RunAsync();