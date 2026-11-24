using System.Security.Cryptography;

namespace TaskDoc.Api.Auth;

/// <summary>
/// PBKDF2-SHA256 password hashing. Stored format: <c>pbkdf2-sha256$iterations$salt$hash</c> (Base64),
/// so the iteration count can be raised later without breaking existing hashes.
/// </summary>
public sealed class PasswordHasher
{
    public const int MinimumLength = 8;
    private const string Scheme = "pbkdf2-sha256";
    private const int SaltSize = 16;
    private const int HashSize = 32;

    // OWASP 2023+ recommendation for PBKDF2-HMAC-SHA256.
    private readonly int _iterations;

    public PasswordHasher() : this(600_000) { }

    /// <summary>Lower iteration counts are only for tests.</summary>
    public PasswordHasher(int iterations) => _iterations = iterations;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, _iterations, HashAlgorithmName.SHA256, HashSize);
        return $"{Scheme}${_iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string storedHash)
    {
        var parts = storedHash.Split('$');
        if (parts.Length != 4 || parts[0] != Scheme || !int.TryParse(parts[1], out var iterations))
        {
            return false;
        }

        var salt = Convert.FromBase64String(parts[2]);
        var expected = Convert.FromBase64String(parts[3]);
        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }

    /// <summary>
    /// Burns the same time as a real verification. Used when the user does not exist,
    /// so response timing does not reveal which user names are registered.
    /// </summary>
    public void VerifyDummy(string password) =>
        Rfc2898DeriveBytes.Pbkdf2(password, new byte[SaltSize], _iterations, HashAlgorithmName.SHA256, HashSize);

    public static string? ValidateStrength(string? password) =>
        string.IsNullOrEmpty(password) || password.Length < MinimumLength
            ? $"パスワードは{MinimumLength}文字以上にしてください。"
            : null;
}
