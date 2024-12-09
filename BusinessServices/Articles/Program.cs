using snowcoreBlog.ServiceDefaults.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

builder.WebHost.UseKestrelHttpsConfiguration();
builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
//builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapDefaultEndpoints();
// app.UseFastEndpoints();

await app.RunAsync();