namespace TaskDoc.Contracts.Health;

/// <summary>Result of <c>GET /api/health</c>.</summary>
/// <param name="Status">"ok" when the API and its database are reachable.</param>
/// <param name="DatabaseVersion">SQLite library version reported by the database.</param>
/// <param name="ServerTime">Server clock at the time of the check (UTC).</param>
public sealed record HealthResponse(string Status, string DatabaseVersion, DateTimeOffset ServerTime);
