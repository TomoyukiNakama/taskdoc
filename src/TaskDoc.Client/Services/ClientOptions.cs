namespace TaskDoc.Client.Services;

/// <summary>Client settings. Each platform head may override them before the app starts.</summary>
public sealed class ClientOptions
{
    /// <summary>Base address of TaskDoc.Api (matches the "http" profile in its launchSettings.json).</summary>
    public Uri ApiBaseAddress { get; set; } = new("http://localhost:5227");

    /// <summary>
    /// Innermost HTTP handler for API calls. Null uses the platform default. Set it for a proxy,
    /// custom certificate validation, or a stub in tests.
    /// </summary>
    public Func<HttpMessageHandler>? HttpMessageHandlerFactory { get; set; }
}
