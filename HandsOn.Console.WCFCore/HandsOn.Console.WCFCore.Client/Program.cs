using HandsOn.Console.WCFCore.Client.HelloService;

Console.WriteLine("HandsOn.Console.WCFCore - Client");

var client = new HelloServiceClient("https://localhost:5001/HelloService");

Console.WriteLine(client.GetHello("World"));

var greetings = client.Greet("Krystian");
Console.WriteLine(greetings);