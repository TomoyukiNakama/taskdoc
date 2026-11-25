using System.Net;
using System.Net.Http.Headers;

namespace TaskDoc.Client.Services;

/// <summary>
/// Adds the access token to API requests. If the API still answers 401, renews the token once and retries.
/// </summary>
public sealed class AuthHeaderHandler(AuthSession session) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var token = await session.GetAccessTokenAsync(ct);
        if (token is null)
        {
            return await base.SendAsync(request, ct);
        }

        // Buffer the body so the request can be sent a second time.
        if (request.Content is not null)
        {
            await request.Content.LoadIntoBufferAsync(ct);
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await base.SendAsync(request, ct);
        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        var renewed = await session.RenewAfterRejectionAsync(token, ct);
        if (renewed is null)
        {
            return response;
        }

        response.Dispose();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", renewed);
        return await base.SendAsync(request, ct);
    }
}
