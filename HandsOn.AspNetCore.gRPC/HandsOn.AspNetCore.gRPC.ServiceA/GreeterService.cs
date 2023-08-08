using Grpc.Core;

namespace HandsOn.AspNetCore.Grpc.ServiceA;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"{nameof(SayHello)} called with '{request.Name}' name parameter");

        return Task.FromResult(new HelloReply
        {
            Message = $"Hello {request.Name}"
        });
    }
}