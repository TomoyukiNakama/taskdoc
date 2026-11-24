using System.Data.Common;

namespace TaskDoc.Api.Data;

/// <summary>
/// Opens database connections for Dapper. Repositories depend on this interface only,
/// so the database engine can be swapped later without touching endpoint code.
/// </summary>
public interface IDbConnectionFactory
{
    Task<DbConnection> OpenAsync(CancellationToken cancellationToken = default);
}
