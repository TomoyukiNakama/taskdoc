using DbUp;
using Microsoft.Data.Sqlite;

namespace TaskDoc.Api.Data;

/// <summary>
/// Applies the embedded scripts in Data/Migrations that have not run yet.
/// DbUp records applied scripts in its SchemaVersions table.
/// </summary>
public static class DatabaseMigrator
{
    public static void Migrate(string connectionString, ILogger logger)
    {
        EnsureDirectoryExists(connectionString);

        var upgrader = DeployChanges.To
            .SqliteDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(
                typeof(DatabaseMigrator).Assembly,
                name => name.Contains(".Data.Migrations.", StringComparison.Ordinal))
            .WithTransactionPerScript()
            .LogToNowhere()
            .Build();

        var scripts = upgrader.GetScriptsToExecute();
        if (scripts.Count == 0)
        {
            logger.LogInformation("Database schema is up to date.");
            return;
        }

        var result = upgrader.PerformUpgrade();
        if (!result.Successful)
        {
            throw new InvalidOperationException(
                $"Database migration failed at script '{result.ErrorScript?.Name}'.", result.Error);
        }

        foreach (var script in result.Scripts)
        {
            logger.LogInformation("Applied migration {Script}", script.Name);
        }
    }

    private static void EnsureDirectoryExists(string connectionString)
    {
        var dataSource = new SqliteConnectionStringBuilder(connectionString).DataSource;
        if (string.IsNullOrEmpty(dataSource) || dataSource == ":memory:")
        {
            return;
        }

        var directory = Path.GetDirectoryName(Path.GetFullPath(dataSource));
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
