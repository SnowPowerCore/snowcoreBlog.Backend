using FastEndpoints;
using Ixnas.AltchaNet;
using Marten;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.HttpOverrides;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using MinimalStepifiedSystem.Extensions;
using snowcoreBlog.Backend.Articles.Repositories.Marten;
using snowcoreBlog.Backend.Articles.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Articles.Steps;
using snowcoreBlog.ServiceDefaults.Extensions;
using JasperFx.CodeGeneration;
using snowcoreBlog.Backend.Core.Entities.Article;
using Oakton;
using snowcoreBlog.ApplicationLaunch.Implementations.BackgroundServices;
using snowcoreBlog.ApplicationLaunch.Interfaces;
using snowcoreBlog.Backend.Articles.Services;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Host.UseDefaultServiceProvider(static (c, options) =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});
builder.Host.ApplyOaktonExtensions();

builder.Services.Configure<JsonOptions>(static options =>
{
    options.SerializerOptions.SetJsonSerializationContext();
});

builder.Services.Configure<CookiePolicyOptions>(static options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.Configure<ForwardedHeadersOptions>(static options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.WebHost.UseKestrelHttpsConfiguration();
builder.AddServiceDefaults();

builder.AddNpgsqlDataSource(connectionName: "db-snowcore-blog-article-entities");
//builder.Services.AddNpgsqlDataSource("Host=localhost;Port=54523;Username=postgres;Password=xQ6S1zf+)!kTnjFFCtt(Ks");

builder.Services.AddMarten(static options =>
{
    options.RegisterDocumentType<ArticleEntity>();
    options.RegisterDocumentType<ArticleSnapshotEntity>();
    options.GeneratedCodeMode = TypeLoadMode.Static;
    options.UseSystemTextJsonForSerialization(configure: static o => o.SetJsonSerializationContext());
    options.Policies.AllDocumentsSoftDeleted();
})
    .UseLightweightSessions()
    .UseNpgsqlDataSource();

builder.Services.AddSingleton(static sp =>
{
    var key = new byte[64];
    using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
    rng.GetBytes(key);
    return Altcha.CreateServiceBuilder()
        .UseSha256(key)
        .SetExpiryInSeconds(30)
        .UseStore(() =>
        {
            using var scope = sp.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IAltchaCancellableChallengeStore>();
        })
        .Build();
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization()
    .AddAntiforgery(options => options.Cookie.Expiration = TimeSpan.Zero)
    .AddFastEndpoints(static options =>
    {
        options.SourceGeneratorDiscoveredTypes.AddRange(snowcoreBlog.Backend.Articles.DiscoveredTypes.All);
    });

builder.Services.AddSingleton<IApplicationLaunchService>(static sp => new ArticlesApplicationLaunchService(sp));
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<ValidateAuthorAccountStep>();
builder.Services.AddScoped<GenerateSlugStep>();
builder.Services.AddScoped<SaveArticleStep>();

builder.Services.AddHostedService(static sp =>
    new ApplicationLaunchWorker(sp.GetRequiredService<IHostApplicationLifetime>(),
        sp.GetRequiredService<IApplicationLaunchService>()));

var app = builder.Build();

app.UseStepifiedSystem();
app.UseHttpsRedirection()
    .UseCookiePolicy(new()
    {
        MinimumSameSitePolicy = SameSiteMode.Strict,
        HttpOnly = HttpOnlyPolicy.Always,
        Secure = CookieSecurePolicy.Always
    })
    .UseAuthentication()
    .UseAuthorization()
    .UseAntiforgeryFE();

app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = default;
    c.Versioning.Prefix = "v";
    c.Serializer.Options.SetJsonSerializationContext();
});

app.MapDefaultEndpoints();

await app.RunOaktonCommands(args);