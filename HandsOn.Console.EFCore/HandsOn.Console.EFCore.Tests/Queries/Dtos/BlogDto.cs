namespace HandsOn.Console.EFCore.Tests.Queries.Dtos;

public class BlogDto
{
    public int BlogId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public ICollection<PostDto> Posts { get; set; }
}