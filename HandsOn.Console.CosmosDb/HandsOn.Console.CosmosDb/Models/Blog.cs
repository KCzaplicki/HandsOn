namespace HandsOn.Console.CosmosDb.Models;

public class Blog
{
    public string id { get; set; }
    public string blogId { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
    public Contact contact { get; set; }
    
    public string type => nameof(Blog);

    public override string ToString()
    {
        return $"Id: {id}, BlogId: {blogId}, Name: {name}, Description: {description}, CreatedAt: {createdAt}, UpdatedAt: {updatedAt}, Contact: {contact}, Type: {type}";
    }
}