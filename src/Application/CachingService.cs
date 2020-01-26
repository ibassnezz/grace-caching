using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace GraceCaching.Application
{
    public class CachingService<TResult>
    {
        private readonly IMemoryCache _memoryCache;
        private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(5);
        private static readonly ConcurrentDictionary<string, TaskCompletionSource<TResult>> _taskCompletionSources = new ConcurrentDictionary<string, TaskCompletionSource<TResult>>();

        public CachingService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<TResult> Execute(Func<Task<TResult>> func, string key)
        {
            var taskCompletionSource = new TaskCompletionSource<TResult>(); ;

            if (_taskCompletionSources.TryAdd(key, taskCompletionSource))
            {
                try
                {
                    if (!_memoryCache.TryGetValue<TResult>(key, out var result))
                    {
                        result = await func();
                        _memoryCache.Set(key, result, new MemoryCacheEntryOptions().SetAbsoluteExpiration(Ttl));
                    }

                    taskCompletionSource.SetResult(result);

                    result = await taskCompletionSource.Task;
                    _taskCompletionSources.TryRemove(key, out _);
                    return result;
                }
                catch (Exception e)
                {
                    taskCompletionSource.SetException(e);
                    _taskCompletionSources.TryRemove(key, out _);
                    throw;
                }
            }
            else
            {
                if (_taskCompletionSources.TryGetValue(key, out taskCompletionSource))
                    return await taskCompletionSource.Task;

                if (_memoryCache.TryGetValue<TResult>(key, out var result))
                    return result;

                return await func();
            }
        }
    }
}