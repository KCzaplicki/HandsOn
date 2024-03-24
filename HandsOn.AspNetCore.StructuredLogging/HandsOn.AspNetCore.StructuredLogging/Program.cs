// Setup kibana with docker on https://www.elastic.co/guide/en/kibana/current/docker.html
// Configure Serilog with Elasticsearch sink on https://github.com/serilog-contrib/serilog-sinks-elasticsearch
using HandsOn.AspNetCore.StructuredLogging.Endpoints;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.MapGet("/", (ILogger<Program> logger) =>
{
    const string appName = "HandsOn.AspNetCore.StructuredLogging";
    logger.LogInformation("{AppName} is running", appName);
    
    return $"{appName} is running!";
});
app.MapEmployeeEndpoints();

app.Run();