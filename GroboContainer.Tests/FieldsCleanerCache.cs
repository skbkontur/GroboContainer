using System;
using System.Collections.Generic;

namespace GroboContainer.Tests
{
    public static class FieldsCleanerCache
    {
        public static void Clean(object obj)
        {
            var type = obj.GetType();
            var key = (long)type.TypeHandle.Value;
            if (!cache.TryGetValue(key, out var action))
            {
                action = new FieldsCleaner(type).GetDelegate();
                cache[key] = action;
            }
            action(obj);
        }

        private static readonly IDictionary<long, Action<object>> cache = new Dictionary<long, Action<object>>();
    }
}