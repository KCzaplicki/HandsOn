using System.Text.Json;

namespace HandsOn.AspNetCore.StructuredLogging.Endpoints;

public static class EmployeeEndpoints
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    
    public static IEndpointRouteBuilder MapEmployeeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/employees", (ILogger<Program> logger) =>
        {
            logger.LogInformation("Getting all employees");

            var employees = new[]
            {
                new { Id = 1, Name = "John Doe" },
                new { Id = 2, Name = "Jane Doe" }
            };
            
            return Results.Json(employees, JsonOptions);
        });

        endpoints.MapGet("/employees/{id}", (string id, ILogger<Program> logger) =>
        {
            logger.LogInformation("Getting employee with ID {Id}", id);

            var employee = new { Id = id, Name = "John Doe" };

            return Results.Json(employee, JsonOptions);
        });

        return endpoints;
    }
}