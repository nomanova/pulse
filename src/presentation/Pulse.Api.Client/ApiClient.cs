using System;

namespace Pulse.Api.Client;

public class ApiClient : IDisposable
{
    protected ApiHttpClient? HttpClient { get; private set; }

    private bool _isDisposed;

    protected ApiClient(ApiClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        HttpClient = ApiHttpClient.Create(options);
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        if (HttpClient != null)
        {
            try
            {
                HttpClient.Dispose();
            }
            catch (Exception)
            {
                // NOP: might happen for inflight request during client disposal
            }

            HttpClient = null;
        }

        _isDisposed = true;
    }
}