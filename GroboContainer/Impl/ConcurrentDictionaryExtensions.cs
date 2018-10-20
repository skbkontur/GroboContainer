using System;
using System.Collections.Concurrent;

namespace GroboContainer.Impl
{
    internal static class ConcurrentDictionaryExtensions
    {
        public static bool TryAddOrUpdate<TKey, TValue>(
            this ConcurrentDictionary<TKey, TValue> source,
            TKey key, TValue value, Func<TValue, bool> updateFilter)
        {
            bool found;
            while ((found = source.TryGetValue(key, out var existingValue)) && updateFilter(existingValue) || !found)
            {
                if (!found && source.TryAdd(key, value) || found && source.TryUpdate(key, value, existingValue))
                    return true;
            }
            return false;
        }
    }
}