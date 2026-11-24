using System.Data;
using System.Globalization;
using Dapper;

namespace TaskDoc.Api.Data;

/// <summary>
/// SQLite has no date type. Timestamps are stored as ISO 8601 UTC text ("O" format),
/// which also sorts correctly as text. Call <see cref="Register"/> once at startup.
/// </summary>
public static class DapperTypeHandlers
{
    private static int _registered;

    public static void Register()
    {
        if (Interlocked.Exchange(ref _registered, 1) == 1)
        {
            return;
        }

        SqlMapper.RemoveTypeMap(typeof(DateTimeOffset));
        SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
    }

    private sealed class DateTimeOffsetHandler : SqlMapper.TypeHandler<DateTimeOffset>
    {
        public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
        {
            parameter.DbType = DbType.String;
            parameter.Value = value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);
        }

        public override DateTimeOffset Parse(object value) =>
            DateTimeOffset.Parse((string)value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
    }
}
