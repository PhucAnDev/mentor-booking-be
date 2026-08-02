using System;
using System.Threading.Tasks;

namespace InfrastructureLayer.Core.Cache
{
    /// <summary>
    /// No-op cache service used when Redis is unavailable.
    /// Allows the application to run without caching instead of crashing.
    /// </summary>
    public class NullCacheService : ICacheService
    {
        public Task<T?> Get<T>(string key) => Task.FromResult<T?>(default);

        public Task Set<T>(string key, T value) => Task.CompletedTask;

        public Task Set<T>(string key, T value, TimeSpan expiration) => Task.CompletedTask;

        public Task<bool> Update<T>(string key, T value) => Task.FromResult(false);

        public Task Remove(string key) => Task.CompletedTask;

        public Task<bool> Exists(string key) => Task.FromResult(false);

        public Task Clear() => Task.CompletedTask;

        public Task ClearWithPattern(string pattern) => Task.CompletedTask;

        public Task ForceLogout(Guid userId) => Task.CompletedTask;
    }
}
