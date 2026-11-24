using System.Net.Http.Json;
using TaskDoc.Contracts.Health;

namespace TaskDoc.Api.Tests;

public sealed class HealthEndpointTests(TaskDocApiFactory factory) : IClassFixture<TaskDocApiFactory>
{
    [Fact]
    public async Task Health_ReportsOkAndSqliteVersion()
    {
        var client = factory.CreateClient();

        var health = await client.GetFromJsonAsync<HealthResponse>(
            "/api/health", TestContext.Current.CancellationToken);

        Assert.NotNull(health);
        Assert.Equal("ok", health.Status);
        Assert.Matches(@"^\d+\.\d+\.\d+", health.DatabaseVersion);
    }
}
