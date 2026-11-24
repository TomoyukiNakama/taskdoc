using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using TaskDoc.Api.Auth;
using TaskDoc.Api.Users;
using TaskDoc.Contracts.Auth;
using TaskDoc.Contracts.Users;

namespace TaskDoc.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", LoginAsync).AllowAnonymous();
        group.MapPost("/refresh", RefreshAsync).AllowAnonymous();
        group.MapPost("/logout", LogoutAsync).AllowAnonymous();
        group.MapGet("/me", GetMeAsync);
        group.MapPut("/me/password", ChangeOwnPasswordAsync);

        return routes;
    }

    private static async Task<Results<Ok<TokenResponse>, ProblemHttpResult>> LoginAsync(
        LoginRequest request, AuthService auth, CancellationToken ct)
    {
        var result = await auth.LoginAsync(request, ct);
        return result.Failure switch
        {
            null => TypedResults.Ok(result.Tokens!),
            LoginFailure.DirectoryUnavailable => TypedResults.Problem(
                "認証サーバー（Active Directory）に接続できません。時間をおいて再度お試しください。",
                statusCode: StatusCodes.Status503ServiceUnavailable),
            _ => TypedResults.Problem(
                "ユーザー名またはパスワードが正しくありません。",
                statusCode: StatusCodes.Status401Unauthorized),
        };
    }

    private static async Task<Results<Ok<TokenResponse>, ProblemHttpResult>> RefreshAsync(
        RefreshRequest request, AuthService auth, CancellationToken ct)
    {
        var tokens = await auth.RefreshAsync(request, ct);
        return tokens is null
            ? TypedResults.Problem("セッションの有効期限が切れました。再度ログインしてください。", statusCode: StatusCodes.Status401Unauthorized)
            : TypedResults.Ok(tokens);
    }

    private static async Task<NoContent> LogoutAsync(LogoutRequest request, AuthService auth, CancellationToken ct)
    {
        await auth.LogoutAsync(request, ct);
        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<UserDto>, UnauthorizedHttpResult>> GetMeAsync(
        ClaimsPrincipal principal, UserRepository users, CancellationToken ct)
    {
        var user = await users.FindByIdAsync(principal.GetUserId(), ct);
        return user is { IsActive: true } ? TypedResults.Ok(user.ToDto()) : TypedResults.Unauthorized();
    }

    /// <summary>
    /// Changes the caller's own password (local users only). Every existing session of the user is
    /// signed out and a fresh session is returned for the caller.
    /// </summary>
    private static async Task<Results<Ok<TokenResponse>, ValidationProblem, ProblemHttpResult, UnauthorizedHttpResult>> ChangeOwnPasswordAsync(
        ChangePasswordRequest request,
        ClaimsPrincipal principal,
        UserRepository users,
        RefreshTokenRepository refreshTokens,
        PasswordHasher passwordHasher,
        AuthService auth,
        TimeProvider clock,
        CancellationToken ct)
    {
        var user = await users.FindByIdAsync(principal.GetUserId(), ct);
        if (user is not { IsActive: true })
        {
            return TypedResults.Unauthorized();
        }

        if (user.AuthType != UserAuthType.Local || user.PasswordHash is null)
        {
            return TypedResults.Problem(
                "AD ユーザーのパスワードは Active Directory で変更してください。", statusCode: StatusCodes.Status400BadRequest);
        }

        if (!passwordHasher.Verify(request.CurrentPassword ?? "", user.PasswordHash))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(ChangePasswordRequest.CurrentPassword)] = ["現在のパスワードが正しくありません。"],
            });
        }

        if (PasswordHasher.ValidateStrength(request.NewPassword) is { } weak)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(ChangePasswordRequest.NewPassword)] = [weak],
            });
        }

        var now = clock.GetUtcNow();
        await users.SetPasswordHashAsync(user.Id, passwordHasher.Hash(request.NewPassword), now, ct);
        await refreshTokens.RevokeAllForUserAsync(user.Id, now, ct);
        return TypedResults.Ok(await auth.StartSessionAsync(user, ct));
    }
}
