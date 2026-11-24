using System.Data.Common;
using Dapper;
using Microsoft.Data.Sqlite;

namespace TaskDoc.Api.Data;

public sealed class SqliteConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public string ConnectionString { get; } = connectionString;

    public async Task<DbConnection> OpenAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqliteConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        // SQLite leaves foreign keys off unless enabled per connection.
        await connection.ExecuteAsync("PRAGMA foreign_keys = ON;");
        return connection;
    }
}
