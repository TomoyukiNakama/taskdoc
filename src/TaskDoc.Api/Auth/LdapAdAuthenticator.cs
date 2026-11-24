using System.DirectoryServices.Protocols;
using System.Net;
using Microsoft.Extensions.Options;

namespace TaskDoc.Api.Auth;

/// <summary>
/// Verifies AD credentials with an LDAP simple bind as <c>account@Domain</c>.
/// On Linux this needs the OpenLDAP client library (libldap) and, for LDAPS,
/// the domain's CA certificate in the system trust store.
/// </summary>
public sealed class LdapAdAuthenticator(IOptions<AuthOptions> options, ILogger<LdapAdAuthenticator> logger)
    : IAdAuthenticator
{
    // LDAP result code 49: invalidCredentials (wrong password, expired / disabled / locked account).
    private const int InvalidCredentialsCode = 49;

    public Task<AdAuthResult> AuthenticateAsync(string accountName, string password, CancellationToken cancellationToken)
    {
        var ad = options.Value.ActiveDirectory;
        if (!ad.Enabled)
        {
            logger.LogWarning("AD sign-in attempted for {Account} but Auth:ActiveDirectory:Enabled is false.", accountName);
            return Task.FromResult(AdAuthResult.Unavailable);
        }

        // An empty password makes an LDAP "unauthenticated bind", which many servers accept. Never allow it.
        if (string.IsNullOrEmpty(password))
        {
            return Task.FromResult(AdAuthResult.InvalidCredentials);
        }

        return Task.Run(() => Bind(ad, accountName, password), cancellationToken);
    }

    private AdAuthResult Bind(ActiveDirectoryOptions ad, string accountName, string password)
    {
        try
        {
            using var connection = new LdapConnection(new LdapDirectoryIdentifier(ad.Server, ad.Port))
            {
                AuthType = AuthType.Basic,
                Timeout = ad.Timeout,
            };
            connection.SessionOptions.ProtocolVersion = 3;
            connection.SessionOptions.SecureSocketLayer = ad.UseSsl;
            connection.SessionOptions.ReferralChasing = ReferralChasingOptions.None;

            connection.Bind(new NetworkCredential($"{accountName}@{ad.Domain}", password));
            return AdAuthResult.Success;
        }
        catch (LdapException ex) when (ex.ErrorCode == InvalidCredentialsCode)
        {
            return AdAuthResult.InvalidCredentials;
        }
        catch (Exception ex) when (ex is LdapException or DirectoryOperationException or DllNotFoundException)
        {
            logger.LogError(ex, "Could not reach Active Directory at {Server}:{Port}.", ad.Server, ad.Port);
            return AdAuthResult.Unavailable;
        }
    }
}
