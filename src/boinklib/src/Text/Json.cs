using System;
using System.Collections.Generic;

using System.Linq;

namespace Boink.Text
{
    public static class Json
    {
        public static string ToJsonString(object obj)
        {
            Type type = obj.GetType();
            if (type == typeof(Dictionary<string, object>))
            {
                return ((Dictionary<string, object>)obj).ToJsonString();
            }
            else if (type.IsGenericType)
            {
                return ((IEnumerable<object>)obj).ToJsonString();
            }

            return obj.ToString().ToJsonString();
        }

        public static string ToJsonString(this IEnumerable<object> enumerable)
        {
            return "[" + string.Join(",", enumerable.Select(v => ToJsonString(v)).ToArray()) + "]";
        }

        public static string ToJsonString(this string str)
        {
            return '"' + str + '"';
        }

        public static string ToJsonString(this Dictionary<string, object> dictionary)
        {
            return "{" + string.Join(",", dictionary.Select(kv => ToJsonString(kv.Key) + ": " + ToJsonString(kv.Value)).ToArray()) + "}";
        }
    }
}