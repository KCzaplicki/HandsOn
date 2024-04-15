using System.ServiceModel;
using HandsOn.Console.WCFCore.Contracts;

namespace HandsOn.Console.WCFCore.Client.HelloService;

public class HelloServiceClient : ClientBase<IHelloService>, IHelloService
{
    public HelloServiceClient(string endpointUri)
        : base(new BasicHttpBinding(BasicHttpSecurityMode.Transport), new EndpointAddress(endpointUri))
    {
    }

    public string GetHello(string name) => Channel.GetHello(name);

    public Greetings Greet(string name) => Channel.Greet(name);
}