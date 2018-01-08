using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace Tests.NMockHelpers
{
    public class SimpleTypeWriter
    {
        private static readonly Dictionary<Type, Func<object, string>> serializers = new Dictionary<Type, Func<object, string>>()
        {
            {
                typeof (string),
                o => (string) o
            },
            {
                typeof (Guid),
                ObjToString
            },
            {
                typeof (NameValueCollection),
                NameValueCollectionToString
            },
            {
                typeof (IPEndPoint),
                ObjToString
            },
            {
                typeof (DateTime),
                o => ((DateTime) o).Ticks.ToString()
            }
        };

        private static string ObjToString(object o)
        {
            return o.ToString();
        }

        private static string NameValueCollectionToString(object o)
        {
            return DumpCollection((NameValueCollection) o);
        }

        public string TryWrite(Type type, object value)
        {
            if (type.IsEnum || type.IsPrimitive)
                return value.ToString();
            if (!serializers.TryGetValue(type, out var func))
                return null;
            return func(value);
        }

        private static string DumpCollection(NameValueCollection collection)
        {
            var collectionSlotList = new List<CollectionSlot>();
            if (collection != null)
            {
                for (int index = 0; index < collection.Count; ++index)
                    collectionSlotList.Add(new CollectionSlot
                    {
                        Key = collection.GetKey(index),
                        Values = collection.GetValues(index)
                    });
            }
            collectionSlotList.Sort((x, y) => string.Compare(x.Key, y.Key));
            var stringBuilder = new StringBuilder();
            foreach (var collectionSlot in collectionSlotList)
                stringBuilder.Append(collectionSlot + "\r\n");
            return stringBuilder.ToString();
        }

        private class CollectionSlot
        {
            public string Key;
            public string[] Values;

            public override string ToString()
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("{'" + Key + "':{");
                if (Values != null)
                {
                    Array.Sort(Values);
                    var flag = false;
                    foreach (var str in Values)
                    {
                        if (flag)
                            stringBuilder.Append(", ");
                        flag = true;
                        stringBuilder.Append("'" + str + "'");
                    }
                }
                stringBuilder.Append("}}");
                return stringBuilder.ToString();
            }
        }
    }
}