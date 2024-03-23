using Elastic.Clients.Elasticsearch;
using FluentAssertions;
using HandsOn.Console.Elasticsearch.Shared.Models;
using HandsOn.Console.Elasticsearch.Shared.Providers;
using HandsOn.Console.Elasticsearch.Tests.Fixtures;

namespace HandsOn.Console.Elasticsearch.Tests;

public class BasicOperationsTests : IClassFixture<ElasticsearchFixture>
{
    private const string IndexName = "employee-index-basic-operations";
    
    private readonly ElasticsearchFixture _fixture;
    private readonly EmployeeFaker _employeeFaker;

    public BasicOperationsTests(ElasticsearchFixture fixture)
    {
        _fixture = fixture;
        _employeeFaker = new EmployeeFaker();
    }

    [Fact]
    public async Task IndexAsync_IndexesDocument()
    {
        var employee = _employeeFaker.Generate();
        
        var response = await _fixture.Client.IndexAsync(employee, IndexName);
        
        response.IsValidResponse.Should().BeTrue();
        response.Index.Should().Be(IndexName);
        response.Result.Should().Be(Result.Created);
        response.Id.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAsync_ReturnsDocument()
    {
        var employee = _employeeFaker.Generate();
        await _fixture.Client.IndexAsync(employee, IndexName);
        
        var response = await _fixture.Client.GetAsync<Employee>(employee.Id, idx => idx.Index(IndexName));

        response.IsValidResponse.Should().BeTrue();
        response.Id.Should().NotBeNull();
        response.Found.Should().BeTrue();
        response.Source.Should().BeEquivalentTo(employee);
    }
    
    [Fact]
    public async Task UpdateAsync_UpdatesDocument()
    {
        var employee = _employeeFaker.Generate();
        await _fixture.Client.IndexAsync(employee, IndexName);
        
        employee.FirstName = "Updated";
        var response = await _fixture.Client.UpdateAsync<Employee, Employee>(IndexName, employee.Id, u => u.Doc(employee));
        
        response.IsValidResponse.Should().BeTrue();
        response.Index.Should().Be(IndexName);
        response.Result.Should().Be(Result.Updated);
        response.Id.Should().NotBeNull();
    }
    
    [Fact]
    public async Task DeleteAsync_DeletesDocument()
    {
        var employee = _employeeFaker.Generate();
        await _fixture.Client.IndexAsync(employee, IndexName);
        
        var response = await _fixture.Client.DeleteAsync(IndexName, employee.Id);
        
        response.IsValidResponse.Should().BeTrue();
        response.Index.Should().Be(IndexName);
        response.Result.Should().Be(Result.Deleted);
        response.Id.Should().NotBeNull();
    }
}