namespace HandsOn.Console.CosmosDb.EFCoreProvider.Models;

public class Contact
{
    public string Email { get; set; }
    public string Phone { get; set; }
    
    public override string ToString()
    {
        return $"Email: {Email}, Phone: {Phone}";
    }
}