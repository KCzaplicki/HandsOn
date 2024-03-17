using Bogus;
using HandsOn.Console.Elasticsearch.Shared.Models;

namespace HandsOn.Console.Elasticsearch.Seed.Providers;

public sealed class EmployeeFaker : Faker<Employee>
{
    public EmployeeFaker()
    {
        RuleFor(e => e.Id, f => f.Random.Guid());
        RuleFor(e => e.FirstName, f => f.Name.FirstName());
        RuleFor(e => e.LastName, f => f.Name.LastName());
        RuleFor(e => e.Department, f => f.Commerce.Department());
        RuleFor(e => e.JobTitle, f => f.Name.JobTitle());
        RuleFor(e => e.Email, f => f.Internet.Email());
    }
}