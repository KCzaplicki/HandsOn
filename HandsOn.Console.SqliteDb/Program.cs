Console.WriteLine("Console application with Sqlite database");

await using var dbContext = new HandsOnDbContext();

Console.WriteLine("Create database");
await dbContext.Database.EnsureDeletedAsync();
await dbContext.Database.EnsureCreatedAsync();

Console.WriteLine("\r\nAdd data to database");
await using var transaction = await dbContext.Database.BeginTransactionAsync();
try
{
    var organization = CreateOrganization();
    Console.WriteLine($"\tAdd organization '{organization.Name}' to database");
    dbContext.Organizations.Add(organization);
    await dbContext.SaveChangesAsync();

    var department = CreateDepartment(organization.Id);
    Console.WriteLine($"\tAdd department '{department.Name}' to database");
    dbContext.Departments.Add(department);
    await dbContext.SaveChangesAsync();

    var employeeJohn = CreateEmployee(department.Id, "John", "Doe");
    Console.WriteLine($"\tAdd employee '{employeeJohn.Name} {employeeJohn.Surname}' to database");
    dbContext.Employees.Add(employeeJohn);
    var employeeWill = CreateEmployee(department.Id, "Will", "Smith");
    Console.WriteLine($"\tAdd employee '{employeeWill.Name} {employeeWill.Surname}' to database");
    dbContext.Employees.Add(employeeWill);
    var employeeLuna = CreateEmployee(department.Id, "Luna", "Border");
    Console.WriteLine($"\tAdd employee '{employeeLuna.Name} {employeeLuna.Surname}' to database");
    dbContext.Employees.Add(employeeLuna);
    await dbContext.SaveChangesAsync();

    Console.WriteLine("\tCommit transaction");
    await transaction.CommitAsync();
}
catch (Exception)
{
    Console.WriteLine("\tRollback transaction");
    await transaction.RollbackAsync();
    throw;
}

Console.WriteLine("\r\nRead data from database");
var employees = await dbContext.Employees
    .Include(e => e.Department)
    .ThenInclude(d => d.Organization)
    .ToListAsync();

foreach (var employee in employees)
{
    Console.WriteLine($"\tEmployee: {employee.Name} {employee.Surname}");
    Console.WriteLine($"\tDepartment: {employee.Department.Name}");
    Console.WriteLine($"\tOrganization: {employee.Department.Organization.Name}\r\n");
}

#region Factory methods

Organization CreateOrganization() =>
    new()
    {
        Name = "HandsOn",
        CreatedOn = DateTime.UtcNow
    };

Department CreateDepartment(int organizationId) =>
    new()
    {
        Name = "IT",
        CreatedOn = DateTime.UtcNow,
        OrganizationId = organizationId
    };

Employee CreateEmployee(int departmentId, string name, string surname) =>
    new()
    {
        Name = name,
        Surname = surname,
        Email = "john.doe@org.com",
        Phone = "+123456789",
        Address = "Street 1",
        City = "City",
        Country = "Country",
        ZipCode = "12345",
        CreatedOn = DateTime.UtcNow,
        DepartmentId = departmentId
    };

#endregion
