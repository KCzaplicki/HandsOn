namespace HandsOn.Console.SqliteDb.Data.Models;

public sealed class Organization
{
    public int Id { get; set; }

    public string Name { get; set; }

    public DateTime CreatedOn { get; set; }
    
    public DateTime? UpdatedOn { get; set; }
    
    public ICollection<Department> Departments { get; set; }
}