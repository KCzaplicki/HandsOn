namespace HandsOn.Console.SqliteDb.Data.Models;

public sealed class Department
{
    public int Id { get; set; }

    public string Name { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime? UpdatedOn { get; set; }
    
    public int OrganizationId { get; set; }
    
    public Organization Organization { get; set; }
    
    public ICollection<Employee> Employees { get; set; }
}