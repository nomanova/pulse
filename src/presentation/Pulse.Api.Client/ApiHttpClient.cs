using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Throw;
using CacheControlHeaderValue = System.Net.Http.Headers.CacheControlHeaderValue;

namespace Pulse.Api.Client;

public sealed class ApiHttpClient : IDisposable
{
    private static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds(5);
    
    private readonly HttpClient _httpClient;

    private bool _disposedValue;

    private ApiHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public static ApiHttpClient Create(ApiClientOptions options)
    {
        options.ThrowIfNull();
        options.EndpointProvider.ThrowIfNull();

        HttpMessageHandler pipeline = new HttpClientHandler();

        var httpClient = new HttpClient(pipeline)
        {
            Timeout = options.RequestTimeout ?? DefaultRequestTimeout
        };

        httpClient.DefaultRequestHeaders.CacheControl =
            new CacheControlHeaderValue { NoCache = true };

        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
        
        return new ApiHttpClient(httpClient);
    }

    public async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        return await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private void ThrowIfDisposed()
    {
        if (!_disposedValue)
        {
            return;
        }

        throw new ObjectDisposedException(nameof(ApiHttpClient));
    }

    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            _httpClient.Dispose();
        }

        _disposedValue = true;
    }
}