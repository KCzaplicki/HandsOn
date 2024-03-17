using Elastic.Clients.Elasticsearch;
using HandsOn.Console.Elasticsearch.Shared.Models;

Console.WriteLine("HandsOn.Console.Elasticsearch - console app");

var client = new ElasticsearchClient();

const string index = "employee_index";
const string searchJobTitle = "Senior*";

var results = await client.SearchAsync<Employee>(s => s
    .Index(index)
    .Query(q => q
        .Wildcard(w => w
            .Field("jobTitle.keyword")
            .Value(searchJobTitle)
        )
    )
);

Console.WriteLine($"Query: JobTile = '{searchJobTitle}'. Found {(results.Total > 0 ? results.Total : 0)} employees.");

if (results.Total > 0)
{
    Console.WriteLine("Results:");
    
    foreach (var result in results.Documents)
    {
        Console.WriteLine($"\t{result.FirstName} {result.LastName}");
    }   
}