using HandsOn.AspNetCore.Grpc.ServiceA;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGet("/", () => "HandsOn.AspNetCore.Grpc.ServiceA");
app.MapGrpcService<GreeterService>();

app.Run();