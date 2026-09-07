using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Pulse.Web.Core.Services.Interfaces;

namespace Pulse.Web.Common.Services;

public sealed class LocalStorageService(IJSRuntime jsRuntime) : ILocalStorageService
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    public ValueTask ClearAsync(CancellationToken cancellationToken = default)
    {
        return jsRuntime.InvokeVoidAsync("localStorage.clear", cancellationToken);
    }

    public async ValueTask<T?> GetItemAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var json = await GetItemAsStringAsync(key, cancellationToken).ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
    }

    public ValueTask<string?> GetItemAsStringAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        return jsRuntime.InvokeAsync<string?>("localStorage.getItem", cancellationToken, key);
    }

    public ValueTask<string?> KeyAsync(int index, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);

        return jsRuntime.InvokeAsync<string?>("localStorage.key", cancellationToken, index);
    }

    public async ValueTask<IEnumerable<string>> KeysAsync(CancellationToken cancellationToken = default)
    {
        var length = await LengthAsync(cancellationToken).ConfigureAwait(false);
        var keys = new List<string>(length);

        for (var index = 0; index < length; index++)
        {
            var key = await KeyAsync(index, cancellationToken).ConfigureAwait(false);

            if (key is not null)
            {
                keys.Add(key);
            }
        }

        return keys;
    }

    public ValueTask<int> LengthAsync(CancellationToken cancellationToken = default)
    {
        return jsRuntime.InvokeAsync<int>("eval", cancellationToken, "localStorage.length");
    }

    public ValueTask RemoveItemAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        return jsRuntime.InvokeVoidAsync("localStorage.removeItem", cancellationToken, key);
    }

    public async ValueTask RemoveItemsAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);

        foreach (var key in keys)
        {
            await RemoveItemAsync(key, cancellationToken).ConfigureAwait(false);
        }
    }

    public ValueTask SetItemAsync<T>(string key, T data, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var json = JsonSerializer.Serialize(data, JsonSerializerOptions);

        return SetItemAsStringAsync(key, json, cancellationToken);
    }

    public ValueTask SetItemAsStringAsync(string key, string data, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(data);

        return jsRuntime.InvokeVoidAsync("localStorage.setItem", cancellationToken, key, data);
    }

    public async ValueTask<bool> ContainKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var item = await GetItemAsStringAsync(key, cancellationToken).ConfigureAwait(false);

        return item is not null;
    }
}