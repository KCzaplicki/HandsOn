namespace HandsOn.Console.CosmosDb.Models;

public class Contact
{
    public string email { get; set; }
    public string phone { get; set; }
    
    public override string ToString()
    {
        return $"Email: {email}, Phone: {phone}";
    }
}