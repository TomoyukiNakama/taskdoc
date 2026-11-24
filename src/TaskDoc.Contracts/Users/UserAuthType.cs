using System.Text.Json.Serialization;

namespace TaskDoc.Contracts.Users;

/// <summary>How a user proves their identity.</summary>
[JsonConverter(typeof(JsonStringEnumConverter<UserAuthType>))]
public enum UserAuthType
{
    /// <summary>Password stored (hashed) in the TaskDoc database.</summary>
    Local,

    /// <summary>Password checked against the on-premises Active Directory.</summary>
    Ad,
}
