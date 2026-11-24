using Dapper;
using TaskDoc.Api.Data;
using TaskDoc.Contracts.Health;

namespace TaskDoc.Api.Endpoints;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/health", GetHealthAsync)
            .WithName("GetHealth")
            .WithTags("Health")
            .AllowAnonymous();

        return routes;
    }

    private static async Task<HealthResponse> GetHealthAsync(
        IDbConnectionFactory connections,
        TimeProvider clock,
        CancellationToken cancellationToken)
    {
        await using var connection = await connections.OpenAsync(cancellationToken);
        var version = await connection.ExecuteScalarAsync<string>(
            new CommandDefinition("SELECT sqlite_version();", cancellationToken: cancellationToken));

        return new HealthResponse("ok", version ?? "unknown", clock.GetUtcNow());
    }
}
