var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcClient<Greeter.GreeterClient>(o =>
{
    o.Address = new Uri(builder.Configuration["GreeterService:Url"]);
});

var app = builder.Build();

app.MapGet("/", () => "HandsOn.AspNetCore.Grpc.ServiceB");
app.MapGet("/greet/{name}", (Greeter.GreeterClient client, string name) =>
{
    var response = client.SayHello(new HelloRequest { Name = name });
    
    return response.Message;
});

app.Run();