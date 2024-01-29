var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => $"HandsOn.AspNetCore.Kubernetes - {Environment.GetEnvironmentVariable("POD_NAME")}");

app.Run();