using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Client.Common;
using Pulse.Api.Shared.Contract;

namespace Pulse.Api.Client.Services;

public abstract class BaseService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = false,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IEndpointProvider? _endpointProvider;
    private readonly ITokenProvider? _tokenProvider;
    private readonly ApiHttpClient? _httpClient;

    protected BaseService(
        IEndpointProvider? endpointProvider,
        ITokenProvider? tokenProvider,
        ApiHttpClient? httpClient)
    {
        _endpointProvider = endpointProvider;
        _tokenProvider = tokenProvider;
        _httpClient = httpClient;
    }

    protected async Task<ApiResult> SendAsync(
        HttpMethod method,
        string uri,
        object? payload = null,
        CancellationToken cancellationToken = default)
    {
        return await SendAsync(method, uri, payload, null, cancellationToken);
    }

    protected async Task<ApiDataResult<T>> SendForDataAsync<T>(
        HttpMethod method,
        string uri,
        object? payload = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        return await SendForDataAsync<T>(method, uri, payload, null, cancellationToken);
    }

    private async Task<ApiResult> SendAsync(
        HttpMethod method,
        string uri,
        object? payload = null,
        Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        if (_httpClient == null)
        {
            return ApiResult.ForFailure(HttpStatusCode.ServiceUnavailable, problem: ServiceUnavailableProblem());
        }

        var absoluteUri = await ToUri(uri);

        var request = new HttpRequestMessage(method, absoluteUri);
        TryAddPayload(request, payload);
        TryAddHeaders(request, headers);

        await TryAddToken(request, _tokenProvider);
        
        HttpResponseMessage response;

        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiResult.ForFailure(HttpStatusCode.ServiceUnavailable, problem: ServiceUnavailableProblem(ex));
        }

        return await ParseResponseAsync(response);
    }

    private async Task<ApiDataResult<T>> SendForDataAsync<T>(
        HttpMethod method,
        string uri,
        object? payload = null,
        Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        if (_httpClient == null)
        {
            return ApiDataResult<T>.ForFailure(HttpStatusCode.ServiceUnavailable, problem: ServiceUnavailableProblem());
        }

        var absoluteUri = await ToUri(uri);

        var request = new HttpRequestMessage(method, absoluteUri);
        TryAddPayload(request, payload);
        TryAddHeaders(request, headers);

        await TryAddToken(request, _tokenProvider);
        
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiDataResult<T>.ForFailure(HttpStatusCode.ServiceUnavailable,
                problem: ServiceUnavailableProblem(ex));
        }

        return await ParseDataResponseAsync<T>(response);
    }

    private static async Task<ApiResult> ParseResponseAsync(HttpResponseMessage response)
    {
        var statusCode = response.StatusCode;
        var headers = GetHeaders(response);

        if (response.IsSuccessStatusCode)
        {
            return ApiResult.ForSuccess(statusCode, headers);
        }

        var error = await TryParseProblemAsync(response);
        return ApiResult.ForFailure(statusCode, headers, error);
    }

    private static async Task<ApiDataResult<T>> ParseDataResponseAsync<T>(HttpResponseMessage response)
    {
        var statusCode = response.StatusCode;
        var headers = GetHeaders(response);

        if (!response.IsSuccessStatusCode)
        {
            var error = await TryParseProblemAsync(response);
            return ApiDataResult<T>.ForFailure(statusCode, headers, error);
        }

        var data = await GetPayloadAsync<T>(response);

        return ApiDataResult<T>.ForSuccess(data, statusCode, headers);
    }

    private static async Task<T?> GetPayloadAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, SerializerOptions);
    }

    private static async Task<Problem?> TryParseProblemAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return string.IsNullOrEmpty(content) ? null : JsonSerializer.Deserialize<Problem>(content, SerializerOptions);
    }

    private static Dictionary<string, string> GetHeaders(HttpResponseMessage response)
    {
        var headers = new Dictionary<string, string>();

        foreach (var (name, values) in response.Headers)
        {
            var value = values.FirstOrDefault();

            if (!string.IsNullOrEmpty(value))
            {
                headers.Add(name, value);
            }
        }

        return headers;
    }

    private static void TryAddPayload(HttpRequestMessage request, object? payload)
    {
        switch (payload)
        {
            case null:
                return;
            case MultipartFormDataContent formDataContent:
                request.Content = formDataContent;
                break;
            default:
            {
                var body = JsonSerializer.Serialize(payload, SerializerOptions);
                request.Content = new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json);
                break;
            }
        }
    }

    private static async Task TryAddToken(HttpRequestMessage request, ITokenProvider? tokenProvider)
    {
        if (tokenProvider == null)
        {
            return;
        }
        
        var token = await tokenProvider.Get();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private static void TryAddHeaders(HttpRequestMessage request, Dictionary<string, string>? headers)
    {
        if (headers == null)
        {
            return;
        }

        foreach (var header in headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }
    }

    private static Problem ServiceUnavailableProblem(Exception? ex = null)
    {
        return new Problem
        {
            Code = "General.ServiceUnavailable",
            Description = ex?.Message ?? "Service is unavailable"
        };
    }

    private async Task<Uri?> ToUri(string path)
    {
        if (_endpointProvider == null)
        {
            return new Uri(path);
        }

        var endpoint = await _endpointProvider.Get();

        try
        {
            var baseUri = new Uri(endpoint!);
            return new Uri(baseUri, path);
        }
        catch (Exception)
        {
            return null;
        }
    }
}