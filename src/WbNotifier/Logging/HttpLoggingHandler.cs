using System.Net.Http.Headers;

namespace WbNotifier.Logging;

public class HttpLoggingHandler : DelegatingHandler
{
    private readonly ILogger<HttpLoggingHandler> _logger;

    public HttpLoggingHandler(ILogger<HttpLoggingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["RequestHeaders"] = ReadHeaders(request.Headers) ?? new Dictionary<string, string>(),
                   ["RequestBody"] = request.Content != null? await request.Content.ReadAsStringAsync(cancellationToken) : string.Empty
               }))
        {
            return await base.SendAsync(request, cancellationToken);   
        }
    }
    
    private Dictionary<string, string>? ReadHeaders(HttpHeaders? headers)
    {
        return headers?.ToDictionary(a => a.Key, a => string.Join(";", a.Value));
    }

}