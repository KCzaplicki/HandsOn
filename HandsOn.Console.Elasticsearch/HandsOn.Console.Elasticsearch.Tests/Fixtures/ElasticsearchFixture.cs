using Elastic.Clients.Elasticsearch;

namespace HandsOn.Console.Elasticsearch.Tests.Fixtures;

public sealed class ElasticsearchFixture
{
    public ElasticsearchClient Client { get; }
    
    public ElasticsearchFixture()
    {
        Client = new ElasticsearchClient();
    }
}