using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.HttpResults;
using TaskDoc.Api.Auth;
using TaskDoc.Api.Users;
using TaskDoc.Contracts.Users;

namespace TaskDoc.Api.Endpoints;

/// <summary>User registration and maintenance. System administrators only.</summary>
public static partial class UserEndpoints
{
    public const string AdminPolicy = "Admin";

    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/users").WithTags("Users").RequireAuthorization(AdminPolicy);

        group.MapGet("/", ListAsync);
        group.MapGet("/{id:long}", GetAsync);
        group.MapPost("/", CreateAsync);
        group.MapPut("/{id:long}", UpdateAsync);
        group.MapPut("/{id:long}/password", SetPasswordAsync);

        return routes;
    }

    private static async Task<Ok<UserDto[]>> ListAsync(UserRepository users, CancellationToken ct) =>
        TypedResults.Ok((await users.ListAsync(ct)).Select(u => u.ToDto()).ToArray());

    private static async Task<Results<Ok<UserDto>, NotFound>> GetAsync(long id, UserRepository users, CancellationToken ct) =>
        await users.FindByIdAsync(id, ct) is { } user ? TypedResults.Ok(user.ToDto()) : TypedResults.NotFound();

    private static async Task<Results<Created<UserDto>, ValidationProblem, ProblemHttpResult>> CreateAsync(
        CreateUserRequest request, UserRepository users, PasswordHasher passwordHasher, TimeProvider clock, CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>();
        if (!UserNamePattern().IsMatch(request.UserName ?? ""))
        {
            errors[nameof(request.UserName)] = ["ユーザー名は半角英数字と . _ - で 1〜64 文字にしてください。"];
        }

        ValidateProfile(request.DisplayName, request.Email, errors);

        if (request.AuthType == UserAuthType.Local && PasswordHasher.ValidateStrength(request.Password) is { } weak)
        {
            errors[nameof(request.Password)] = [weak];
        }
        else if (request.AuthType == UserAuthType.Ad && !string.IsNullOrEmpty(request.Password))
        {
            errors[nameof(request.Password)] = ["AD ユーザーにはパスワードを設定しません（AD のパスワードでログインします）。"];
        }

        if (errors.Count > 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        if (await users.FindByUserNameAsync(request.UserName!, ct) is not null)
        {
            return TypedResults.Problem($"ユーザー名 '{request.UserName}' は既に登録されています。", statusCode: StatusCodes.Status409Conflict);
        }

        var hash = request.AuthType == UserAuthType.Local ? passwordHasher.Hash(request.Password!) : null;
        var id = await users.InsertAsync(
            request.UserName!, request.DisplayName.Trim(), NullIfBlank(request.Email), request.AuthType,
            hash, request.IsAdmin, clock.GetUtcNow(), ct);

        var created = (await users.FindByIdAsync(id, ct))!.ToDto();
        return TypedResults.Created($"/api/users/{id}", created);
    }

    private static async Task<Results<Ok<UserDto>, NotFound, ValidationProblem, ProblemHttpResult>> UpdateAsync(
        long id, UpdateUserRequest request, UserRepository users, RefreshTokenRepository refreshTokens,
        TimeProvider clock, CancellationToken ct)
    {
        var user = await users.FindByIdAsync(id, ct);
        if (user is null)
        {
            return TypedResults.NotFound();
        }

        var errors = new Dictionary<string, string[]>();
        ValidateProfile(request.DisplayName, request.Email, errors);
        if (errors.Count > 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        var losesAdmin = user is { IsAdmin: true, IsActive: true } && (!request.IsAdmin || !request.IsActive);
        if (losesAdmin && await users.CountActiveAdminsAsync(ct) <= 1)
        {
            return TypedResults.Problem(
                "有効な管理者が 1 人もいなくなるため、この変更はできません。", statusCode: StatusCodes.Status409Conflict);
        }

        var now = clock.GetUtcNow();
        await users.UpdateProfileAsync(id, request.DisplayName.Trim(), NullIfBlank(request.Email), request.IsAdmin, request.IsActive, now, ct);

        if (user.IsActive && !request.IsActive)
        {
            // Deactivated users cannot refresh; their current access token still works until it expires (15 min by default).
            await refreshTokens.RevokeAllForUserAsync(id, now, ct);
        }

        return TypedResults.Ok((await users.FindByIdAsync(id, ct))!.ToDto());
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem, ProblemHttpResult>> SetPasswordAsync(
        long id, SetPasswordRequest request, UserRepository users, RefreshTokenRepository refreshTokens,
        PasswordHasher passwordHasher, TimeProvider clock, CancellationToken ct)
    {
        var user = await users.FindByIdAsync(id, ct);
        if (user is null)
        {
            return TypedResults.NotFound();
        }

        if (user.AuthType != UserAuthType.Local)
        {
            return TypedResults.Problem(
                "AD ユーザーのパスワードは Active Directory で管理されています。", statusCode: StatusCodes.Status400BadRequest);
        }

        if (PasswordHasher.ValidateStrength(request.NewPassword) is { } weak)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]> { [nameof(request.NewPassword)] = [weak] });
        }

        var now = clock.GetUtcNow();
        await users.SetPasswordHashAsync(id, passwordHasher.Hash(request.NewPassword), now, ct);
        await refreshTokens.RevokeAllForUserAsync(id, now, ct);
        return TypedResults.NoContent();
    }

    private static void ValidateProfile(string? displayName, string? email, Dictionary<string, string[]> errors)
    {
        if (string.IsNullOrWhiteSpace(displayName) || displayName.Trim().Length > 100)
        {
            errors["DisplayName"] = ["表示名は 1〜100 文字で入力してください。"];
        }

        if (!string.IsNullOrWhiteSpace(email) && (email.Length > 256 || !email.Contains('@')))
        {
            errors["Email"] = ["メールアドレスの形式が正しくありません。"];
        }
    }

    private static string? NullIfBlank(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    [GeneratedRegex("^[A-Za-z0-9._-]{1,64}$")]
    private static partial Regex UserNamePattern();
}
