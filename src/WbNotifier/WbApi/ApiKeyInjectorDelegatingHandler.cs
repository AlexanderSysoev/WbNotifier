using Microsoft.AspNetCore.WebUtilities;
using WbNotifier.Settings;

namespace WbNotifier.WbApi;

public class ApiKeyInjectorDelegatingHandler : DelegatingHandler
{
    private readonly WbStatsApiSettings _settings;

    public ApiKeyInjectorDelegatingHandler(WbStatsApiSettings settings)
    {
        _settings = settings;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var uri = QueryHelpers.AddQueryString(request.RequestUri!.ToString(), "key", _settings.Token);
        request.RequestUri = new Uri(uri);
        return await base.SendAsync(request, cancellationToken);
    }
}