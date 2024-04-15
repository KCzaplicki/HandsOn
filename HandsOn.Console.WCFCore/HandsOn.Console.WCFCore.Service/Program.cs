using HandsOn.Console.WCFCore.Contracts;
using HandsOn.Console.WCFCore.Service.Services;

var builder = WebApplication.CreateBuilder();

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<HelloService>();
    serviceBuilder.AddServiceEndpoint<HelloService, IHelloService>(new BasicHttpBinding(BasicHttpSecurityMode.Transport), "/HelloService");
    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpsGetEnabled = true;
});

app.Run();
