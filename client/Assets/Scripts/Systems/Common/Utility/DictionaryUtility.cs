/**************************************************************************/
/*! @file   DictionaryUtility.cs
    @brief  辞書ユーティリティ
***************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics.CodeAnalysis;

namespace EG
{
    public static class DictionaryUtility
    {
        /// <summary>ConcurrentDictionary.GetOrAdd()と同じ機能</summary>
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> factory)
        {
            if (dict == null) { throw new ArgumentNullException(nameof(dict)); }
            if (factory == null) { throw new ArgumentNullException(nameof(factory)); }

            TValue value;
            if (dict.TryGetValue(key, out value))
            {
                return value;
            }

            value = factory(key);
            dict.Add(key, value);
            return value;
        }

        /// <summary>ConcurrentDictionary.GetOrAdd()と同じ機能</summary>
        public static TValue GetOrAdd<TKey, TValue, TContext>(this Dictionary<TKey, TValue> dict, TKey key, Func<TKey, TContext, TValue> factory, TContext context)
        {
            if (dict == null) { throw new ArgumentNullException(nameof(dict)); }
            if (factory == null) { throw new ArgumentNullException(nameof(factory)); }

            TValue value;
            if (dict.TryGetValue(key, out value))
            {
                return value;
            }

            value = factory(key, context);
            dict.Add(key, value);
            return value;
        }

        /// <summary>分割宣言用Extension</summary>
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
        {
            // https://ufcpp.net/study/csharp/datatype/deconstruction/
            key = pair.Key;
            value = pair.Value;
        }

        public static TValue GetOrNull<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
            where TValue : class
        {
            if (dict == null) { throw new ArgumentNullException(nameof(dict)); }

            if (dict.TryGetValue(key, out var value))
            {
                return value;
            }

            return null;
        }
    }

    public static class ValueDictionaryUtility
    {
        public static TValue? GetOrNull<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
            where TValue : struct
        {
            if (dict == null) { throw new ArgumentNullException(nameof(dict)); }

            if (dict.TryGetValue(key, out var value))
            {
                return value;
            }

            return null;
        }

        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default)
            where TValue : struct
        {
            if (dict == null) { throw new ArgumentNullException(nameof(dict)); }

            if (dict.TryGetValue(key, out var value))
            {
                return value;
            }

            return defaultValue;
        }
    }
}
