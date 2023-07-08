namespace HandsOn.Console.SqliteDb.Data.Models;

public sealed class Employee
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Surname { get; set; }
    
    public string Email { get; set; }
    
    public string Phone { get; set; }
    
    public string Address { get; set; }
    
    public string City { get; set; }
    
    public string Country { get; set; }
    
    public string ZipCode { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime? UpdatedOn { get; set; }

    public int DepartmentId { get; set; }
    
    public Department Department { get; set; }
}