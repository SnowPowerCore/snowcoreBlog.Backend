using snowcoreBlog.ServiceDefaults.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddServiceDefaults();
//builder.Services.AddFastEndpoints();

var app = builder.Build();

app.MapDefaultEndpoints();
// app.UseFastEndpoints();

await app.RunAsync();