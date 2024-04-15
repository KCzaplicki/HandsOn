using HandsOn.Console.WCFCore.Contracts;

namespace HandsOn.Console.WCFCore.Service.Services;

public class HelloService : IHelloService
{
    public string GetHello(string name) => $"Hello, {name}!";

    public Greetings Greet(string name) =>
        new()
        {
            Name = name,
            WelcomeText = $"Hello, {name}!"
        };
}