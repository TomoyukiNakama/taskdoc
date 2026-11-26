using System.Text.Json;
using System.Text.Json.Serialization;
using TaskDoc.Contracts.Auth;
using TaskDoc.Contracts.Health;
using TaskDoc.Contracts.Users;

namespace TaskDoc.Contracts;

/// <summary>
/// Source-generated JSON metadata for every DTO sent over the API.
/// Refit's generated clients require it on the client (no reflection, so it also works trimmed on Browser/WASM and mobile).
/// Add a [JsonSerializable] line for each new request/response type.
/// </summary>
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(HealthResponse))]
[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(RefreshRequest))]
[JsonSerializable(typeof(LogoutRequest))]
[JsonSerializable(typeof(TokenResponse))]
[JsonSerializable(typeof(UserDto))]
[JsonSerializable(typeof(UserDto[]))]
[JsonSerializable(typeof(CreateUserRequest))]
[JsonSerializable(typeof(UpdateUserRequest))]
[JsonSerializable(typeof(SetPasswordRequest))]
[JsonSerializable(typeof(ChangePasswordRequest))]
public sealed partial class TaskDocJsonContext : JsonSerializerContext;
