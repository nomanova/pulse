using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Pulse.Api.Client.Handlers;
using Throw;
using CacheControlHeaderValue = System.Net.Http.Headers.CacheControlHeaderValue;

namespace Pulse.Api.Client;

public sealed class ApiHttpClient : IDisposable
{
    private readonly HttpClient _httpClient;

    private bool _disposedValue;

    private ApiHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public static ApiHttpClient Create(ApiClientOptions options)
    {
        options.ThrowIfNull();
        options.ApiEndpointProvider.ThrowIfNull();

        HttpMessageHandler pipeline = new HttpClientHandler();

        if (options.BearerTokenProvider != null)
        {
            pipeline = pipeline.DecorateWith(new BearerTokenRequestHandler(options.BearerTokenProvider));
        }

        var httpClient = new HttpClient(pipeline)
        {
            Timeout = options.RequestTimeout ?? TimeSpan.FromSeconds(5)
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