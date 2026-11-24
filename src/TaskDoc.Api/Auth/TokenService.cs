using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TaskDoc.Api.Users;

namespace TaskDoc.Api.Auth;

public sealed class TokenService(IOptions<AuthOptions> options, SigningKeyProvider signingKey, TimeProvider clock)
{
    public const string AdminRole = "admin";
    public const string DisplayNameClaim = "display_name";

    private readonly JsonWebTokenHandler _handler = new();

    public (string Token, DateTimeOffset ExpiresAt) CreateAccessToken(UserRecord user)
    {
        var jwt = options.Value.Jwt;
        var now = clock.GetUtcNow();
        var expiresAt = now + jwt.AccessTokenLifetime;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Name, user.UserName),
            new(DisplayNameClaim, user.DisplayName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
        };
        if (user.IsAdmin)
        {
            claims.Add(new Claim("role", AdminRole));
        }

        var token = _handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            Subject = new ClaimsIdentity(claims),
            IssuedAt = now.UtcDateTime,
            NotBefore = now.UtcDateTime,
            Expires = expiresAt.UtcDateTime,
            SigningCredentials = new SigningCredentials(signingKey.Key, SecurityAlgorithms.HmacSha256),
        });

        return (token, expiresAt);
    }

    /// <summary>Returns a random opaque refresh token and the hash to store for it.</summary>
    public static (string Token, string Hash) CreateRefreshToken()
    {
        var token = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));
        return (token, HashRefreshToken(token));
    }

    public static string HashRefreshToken(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}

/// <summary>Resolves the JWT signing key once at startup.</summary>
public sealed class SigningKeyProvider
{
    public SigningKeyProvider(IOptions<AuthOptions> options, IHostEnvironment environment, ILogger<SigningKeyProvider> logger)
    {
        var configured = options.Value.Jwt.SigningKey;
        if (!string.IsNullOrEmpty(configured))
        {
            if (configured.Length < 32)
            {
                throw new InvalidOperationException("Auth:Jwt:SigningKey must be at least 32 characters.");
            }

            Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configured));
            return;
        }

        if (!environment.IsDevelopment())
        {
            throw new InvalidOperationException(
                "Auth:Jwt:SigningKey is not configured. Set the Auth__Jwt__SigningKey environment variable.");
        }

        // Development only: a per-process key. Access tokens stop working after a restart;
        // clients recover automatically with their refresh token.
        logger.LogWarning("Auth:Jwt:SigningKey is not set; using a temporary development key.");
        Key = new SymmetricSecurityKey(RandomNumberGenerator.GetBytes(32));
    }

    public SymmetricSecurityKey Key { get; }
}
