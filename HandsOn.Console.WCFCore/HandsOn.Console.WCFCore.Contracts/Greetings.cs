using System.Runtime.Serialization;

namespace HandsOn.Console.WCFCore.Contracts;

[DataContract]
public class Greetings
{
    [DataMember]
    public string Name { get; set; }
    
    [DataMember]
    public string WelcomeText { get; set; }
    
    public override string ToString() => $"{{ \"name\": \"{Name}\", \"welcomeText\": \"{WelcomeText}\" }}";
}