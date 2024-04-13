using BenchmarkDotNet.Attributes;
using HandsOn.Console.EFCoreDeleteBenchmark.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace HandsOn.Console.EFCoreDeleteBenchmark;

[SimpleJob]
public class EFCoreDeleteBenchmark
{
    private const int Count = 20_000;
    
    [Benchmark]
    public async Task RemoveRange()
    {
        await using var sampleDbContext = new SampleDbContext();
        var transaction = await sampleDbContext.Database.BeginTransactionAsync();
        
        var blogs = await sampleDbContext.Blogs.Take(Count).ToListAsync();
        sampleDbContext.Blogs.RemoveRange(blogs);
        
        await sampleDbContext.SaveChangesAsync();
        await transaction.RollbackAsync();
    }
    
    [Benchmark]
    public async Task ExecuteDelete()
    {
        await using var sampleDbContext = new SampleDbContext();
        var transaction = await sampleDbContext.Database.BeginTransactionAsync();
        
        await sampleDbContext.Blogs.Take(Count).ExecuteDeleteAsync();
        
        await transaction.RollbackAsync();
    }
    
    [Benchmark]
    public async Task BulkDelete()
    {
        await using var sampleDbContext = new SampleDbContext();
        var transaction = await sampleDbContext.Database.BeginTransactionAsync();
        
        var blogs = await sampleDbContext.Blogs.Take(Count).ToListAsync();
        await sampleDbContext.BulkDeleteAsync(blogs);
        
        await transaction.RollbackAsync();
    }
    
    [Benchmark]
    public async Task ExecuteSqlInterpolated()
    {
        await using var sampleDbContext = new SampleDbContext();
        var transaction = await sampleDbContext.Database.BeginTransactionAsync();
        
        await sampleDbContext.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM Blogs WHERE Id <= {Count}");
        
        await transaction.RollbackAsync();
    }
}