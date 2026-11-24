using TaskDoc.Api.Auth;

namespace TaskDoc.Api.Tests;

public sealed class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new(iterations: 1_000);

    [Fact]
    public void Verify_AcceptsCorrectPassword_RejectsOthers()
    {
        var hash = _hasher.Hash("correct horse");

        Assert.True(_hasher.Verify("correct horse", hash));
        Assert.False(_hasher.Verify("Correct horse", hash));
        Assert.False(_hasher.Verify("", hash));
    }

    [Fact]
    public void Hash_IsSaltedAndSelfDescribing()
    {
        var a = _hasher.Hash("same password");
        var b = _hasher.Hash("same password");

        Assert.NotEqual(a, b);
        Assert.StartsWith("pbkdf2-sha256$1000$", a);
    }

    [Fact]
    public void Verify_UsesIterationCountStoredInHash()
    {
        var oldHash = new PasswordHasher(iterations: 500).Hash("pw-12345678");

        Assert.True(_hasher.Verify("pw-12345678", oldHash));
    }

    [Fact]
    public void Verify_RejectsMalformedHash() => Assert.False(_hasher.Verify("x", "not-a-hash"));
}
