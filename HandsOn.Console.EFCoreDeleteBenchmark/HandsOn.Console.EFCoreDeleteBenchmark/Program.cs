using BenchmarkDotNet.Running;
using HandsOn.Console.EFCoreDeleteBenchmark;
using HandsOn.Console.EFCoreDeleteBenchmark.DataAccess;
using HandsOn.Console.EFCoreDeleteBenchmark.DataAccess.Seeders;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("HandsOn.Console.EFCoreDeleteBenchmark - EF core delete benchmark");

//await BlogSeeder.Seed(100_000);

await using var sampleDbContext = new SampleDbContext();
var blogCount = await sampleDbContext.Blogs.CountAsync();

Console.WriteLine($"Blog entity count: {blogCount}\n");

BenchmarkRunner.Run<EFCoreDeleteBenchmark>();