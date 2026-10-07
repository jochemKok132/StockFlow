using System.Collections.Concurrent;

namespace StockFlow.CashRegister.Authentication
{
    public class SessionStorageService
    {
        private readonly ConcurrentDictionary<string, string> _storage = new();

        public Task SetItemAsync(string key, string value)
        {
            _storage[key] = value;
            return Task.CompletedTask;
        }

        public Task<string?> GetItemAsync(string key)
        {
            _storage.TryGetValue(key, out string? value);
            return Task.FromResult(value);
        }

        public Task RemoveItemAsync(string key)
        {
            _storage.TryRemove(key, out _);
            return Task.CompletedTask;
        }
    }
}