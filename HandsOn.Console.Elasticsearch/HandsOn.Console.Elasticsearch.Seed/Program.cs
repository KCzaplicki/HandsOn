using Elastic.Clients.Elasticsearch;
using HandsOn.Console.Elasticsearch.Shared.Providers;

Console.WriteLine("HandsOn.Console.Elasticsearch.Seed - seed data app");

var client = new ElasticsearchClient();

const string index = "employee-index";
const int employeeCount = 10_000;

var indexResponse = await client.Indices.CreateAsync(index);
Console.WriteLine(indexResponse.IsValidResponse ? $"Created index: {index}" : $"Failed to create index: {index}");

var employeeFaker = new EmployeeFaker();

for (var i = 0; i < employeeCount; i++)
{
    var employee = employeeFaker.Generate();
    var response = await client.IndexAsync(employee, index);

    Console.WriteLine(response.IsValidResponse
        ? $"Indexed employee: {employee.FirstName} {employee.LastName}"
        : $"Failed to index employee: {employee.FirstName} {employee.LastName}");
}

Console.WriteLine($"\n\nSeeded {employeeCount} employees");