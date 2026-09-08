using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pulse.Web.Core.Services.Interfaces;

public interface ILocalStorage
{
    ValueTask ClearAsync(CancellationToken cancellationToken = default);

    ValueTask<T?> GetItemAsync<T>(string key, CancellationToken cancellationToken = default);

    ValueTask<string?> GetItemAsStringAsync(string key, CancellationToken cancellationToken = default);

    ValueTask<string?> KeyAsync(int index, CancellationToken cancellationToken = default);

    ValueTask<IEnumerable<string>> KeysAsync(CancellationToken cancellationToken = default);

    ValueTask<int> LengthAsync(CancellationToken cancellationToken = default);

    ValueTask RemoveItemAsync(string key, CancellationToken cancellationToken = default);

    ValueTask RemoveItemsAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);

    ValueTask SetItemAsync<T>(string key, T data, CancellationToken cancellationToken = default);

    ValueTask SetItemAsStringAsync(string key, string data, CancellationToken cancellationToken = default);

    ValueTask<bool> ContainKeyAsync(string key, CancellationToken cancellationToken = default);
}