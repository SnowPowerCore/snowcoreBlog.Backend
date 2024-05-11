using FastEndpoints;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseFastEndpoints();

await app.RunAsync();