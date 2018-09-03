using System;
using System.Collections.Concurrent;

namespace GroboContainer.Impl
{
    public static class ConcurrentDictionaryExtensions
    {
        public static bool TryAddOrUpdate<TKey, TValue>(
            this ConcurrentDictionary<TKey, TValue> dict,
            TKey key, TValue value, Func<TValue, bool> updateFilter)
        {
            TValue curValue;
            bool found;
            while ((found = dict.TryGetValue(key, out curValue)) && updateFilter(curValue) || !found)
            {
                if ((!found && dict.TryAdd(key, value)) || (found && dict.TryUpdate(key, value, curValue)))
                    return true;
            }
            return false;
        }
    }
}