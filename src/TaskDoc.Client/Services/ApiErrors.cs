using System.Net;
using System.Text.Json;
using Refit;

namespace TaskDoc.Client.Services;

/// <summary>
/// Turns API failures into messages for the screen. The API writes its problem "detail" and
/// validation "errors" in Japanese, so those are shown as-is.
/// </summary>
public static class ApiErrors
{
    public const string ConnectionFailed = "サーバーに接続できません。ネットワークとサーバーの状態を確認してください。";

    /// <returns>A message, or null if <paramref name="ex"/> is not an API failure the screen should handle.</returns>
    public static string? ToMessage(Exception ex) => ex switch
    {
        ApiException api => FromResponse(api),
        ApiRequestException => ConnectionFailed,
        _ => null,
    };

    private static string FromResponse(ApiException ex)
    {
        var fromBody = FromProblemJson(ex.Content);
        if (fromBody is not null)
        {
            return fromBody;
        }

        return ex.StatusCode switch
        {
            HttpStatusCode.Unauthorized => "ログインの有効期限が切れました。再度ログインしてください。",
            HttpStatusCode.Forbidden => "この操作を行う権限がありません。",
            HttpStatusCode.NotFound => "対象のデータが見つかりません。画面を更新してください。",
            _ => $"処理に失敗しました（{(int)ex.StatusCode}）。",
        };
    }

    /// <summary>Reads an RFC 7807 body: validation "errors" first (one line each), otherwise "detail".</summary>
    internal static string? FromProblemJson(string? content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return null;
        }

        try
        {
            using var json = JsonDocument.Parse(content);
            var root = json.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
            {
                var messages = errors.EnumerateObject()
                    .SelectMany(field => field.Value.ValueKind == JsonValueKind.Array
                        ? field.Value.EnumerateArray().Select(m => m.GetString())
                        : [])
                    .Where(m => !string.IsNullOrEmpty(m))
                    .ToList();
                if (messages.Count > 0)
                {
                    return string.Join(Environment.NewLine, messages);
                }
            }

            return root.TryGetProperty("detail", out var detail) && detail.ValueKind == JsonValueKind.String
                ? detail.GetString()
                : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
