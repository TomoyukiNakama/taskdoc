namespace TaskDoc.Api.Auth;

/// <summary>Bound from the "Auth" configuration section.</summary>
public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    public JwtOptions Jwt { get; set; } = new();
    public LockoutOptions Lockout { get; set; } = new();
    public ActiveDirectoryOptions ActiveDirectory { get; set; } = new();
    public InitialAdminOptions InitialAdmin { get; set; } = new();
}

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "TaskDoc";
    public string Audience { get; set; } = "TaskDoc";

    /// <summary>
    /// HMAC-SHA256 key, at least 32 characters. Required outside Development
    /// (set it with the Auth__Jwt__SigningKey environment variable, never in a committed file).
    /// </summary>
    public string? SigningKey { get; set; }

    public TimeSpan AccessTokenLifetime { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(14);
}

/// <summary>Locks an account after repeated failed sign-ins (local and AD users).</summary>
public sealed class LockoutOptions
{
    public int MaxFailedAttempts { get; set; } = 5;
    public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(15);
}

/// <summary>On-premises Active Directory reached over LDAP.</summary>
public sealed class ActiveDirectoryOptions
{
    public bool Enabled { get; set; }

    /// <summary>Domain controller host name, e.g. "dc01.corp.example.com".</summary>
    public string Server { get; set; } = "";

    /// <summary>636 for LDAPS (default), 389 for plain LDAP.</summary>
    public int Port { get; set; } = 636;

    /// <summary>
    /// Use LDAPS. Keep this on: the bind sends the user's password, which travels in clear text over plain LDAP.
    /// </summary>
    public bool UseSsl { get; set; } = true;

    /// <summary>UPN suffix appended to the account name for the bind, e.g. "corp.example.com".</summary>
    public string Domain { get; set; } = "";

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
}

/// <summary>Created on startup when the Users table is empty, so the first admin can sign in.</summary>
public sealed class InitialAdminOptions
{
    public string UserName { get; set; } = "admin";
}
