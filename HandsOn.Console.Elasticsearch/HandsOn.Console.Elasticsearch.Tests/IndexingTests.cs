using Bogus;
using FluentAssertions;
using HandsOn.Console.Elasticsearch.Tests.Fixtures;

namespace HandsOn.Console.Elasticsearch.Tests;

// More about indices on
// https://www.elastic.co/guide/en/elasticsearch/reference/current/indices.html
public class IndexingTests : IClassFixture<ElasticsearchFixture>
{
    private readonly ElasticsearchFixture _fixture;

    public IndexingTests(ElasticsearchFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task IndicesCreateAsync_CreatesIndex()
    {
        var indexName = new Randomizer().Words(1).ToLower();

        var response = await _fixture.Client.Indices.CreateAsync(indexName);
        
        response.IsValidResponse.Should().BeTrue();
        response.Index.Should().Be(indexName);
    }

    // More about name restrictions:
    // https://www.elastic.co/guide/en/elasticsearch/reference/current/indices-create-index.html#indices-create-api-request-body
    [Theory]
    [InlineData("Uppercase")]
    [InlineData("special-characters-!@#$%^&*()")]
    public async Task IndicesCreateAsync_HasNameRestrictions(string indexName)
    {
        var response = await _fixture.Client.Indices.CreateAsync(indexName);
        
        response.IsValidResponse.Should().BeFalse();
        response.ElasticsearchServerError!.Status.Should().Be(400);
        response.ElasticsearchServerError.Error.Type.Should().Be("invalid_index_name_exception");
    }
    
    [Fact]
    public async Task IndicesCreateAsync_CantCreateIndex_WhenIndexAlreadyExists()
    {
        var indexName = new Randomizer().Words(1).ToLower();

        var response = await _fixture.Client.Indices.CreateAsync(indexName);
        response.IsValidResponse.Should().BeTrue();

        response = await _fixture.Client.Indices.CreateAsync(indexName);
        response.IsValidResponse.Should().BeFalse();
        response.ElasticsearchServerError!.Status.Should().Be(400);
        response.ElasticsearchServerError.Error.Type.Should().Be("resource_already_exists_exception");
    }
}