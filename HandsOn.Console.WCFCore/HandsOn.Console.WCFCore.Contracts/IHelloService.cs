using System.ServiceModel;

namespace HandsOn.Console.WCFCore.Contracts;

[ServiceContract]
public interface IHelloService
{
    [OperationContract]
    string GetHello(string name);
    
    [OperationContract]
    Greetings Greet(string name);
}