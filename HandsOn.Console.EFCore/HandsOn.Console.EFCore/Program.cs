Console.WriteLine("Console Application with EF Core and SQL Server");

await using var dbContext = new HandsOnDbContext();

await dbContext.Database.EnsureDeletedAsync();
await dbContext.Database.EnsureCreatedAsync();

Console.WriteLine("HandsOnDb database created");