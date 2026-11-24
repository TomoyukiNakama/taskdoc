namespace TaskDoc.Api.Auth;

public enum AdAuthResult
{
    Success,
    InvalidCredentials,

    /// <summary>The directory could not be reached or AD sign-in is disabled.</summary>
    Unavailable,
}

/// <summary>Checks an AD account's password. Replaced by a fake in tests.</summary>
public interface IAdAuthenticator
{
    /// <param name="accountName">sAMAccountName as registered in the Users table.</param>
    Task<AdAuthResult> AuthenticateAsync(string accountName, string password, CancellationToken cancellationToken);
}
