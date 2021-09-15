using UnityEngine;
using System.Collections;
using System;
using System.Text;
using Farmhash.Sharp;
using CSharpHelpers;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;

namespace EG
{
    public static class StringUtility
    {
        public static char[] PathSeperators = new char[] { '/', '\\' };
        public static char[] sharedBuffer;
        static System.Globalization.CompareInfo compareInfo = CultureInfo.CurrentCulture.CompareInfo;

        #region 
        

        public static int ComputeFarmHash32(this StringBuilder sb)
        {
            if (sb == null) throw new ArgumentNullException(nameof(sb));

            var len = sb.Length;
            var chars = ArrayPool<char>.Shared.Rent(len);
            sb.CopyTo(0, chars, 0, len);
            var hash = Farmhash.Sharp.Farmhash.Hash32(chars, len);
            ArrayPool<char>.Shared.Return(chars);

            return (int)hash;
        }
        
        #endregion 
        
        #region 

        public static string RemoveSurrogate(this string value)
        {
            var isRemoved = false;
            var sb = FastStringBuilder.Alloc();
            for (var i = 0; i < value.Length; ++i)
            {
                var c = value[i];
                if (char.IsSurrogate(c))
                {
                    isRemoved = true;
                }
                else
                {
                    sb.Append(c);
                }
            }

            var s = isRemoved ? sb.ToString() : value;
            FastStringBuilder.Free(sb);

            return s;
        }


        public static string TakeCharacters(this string value, string chars)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var isRemoved = false;
            var sb = FastStringBuilder.Alloc();
            for (var i = 0; i < value.Length; ++i)
            {
                var c = value[i];
                if (chars.IndexOf(c) > -1)
                {
                    sb.Append(c);
                }
                else
                {
                    isRemoved = true;
                }
            }

            var s = isRemoved ? sb.ToString() : value;
            FastStringBuilder.Free(sb);

            return s;
        }
        
        public static string TakeCharacters(this string value, Func<char, bool> predicate)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var isRemoved = false;
            var sb = FastStringBuilder.Alloc();
            for (var i = 0; i < value.Length; ++i)
            {
                var c = value[i];
                if (predicate(c))
                {
                    sb.Append(c);
                }
                else
                {
                    isRemoved = true;
                }
            }

            var s = isRemoved ? sb.ToString() : value;
            FastStringBuilder.Free(sb);

            return s;
        }
        
        public static string RemoveCharacters(this string value, string chars)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var isRemoved = false;
            var sb = FastStringBuilder.Alloc();
            for (var i = 0; i < value.Length; ++i)
            {
                var c = value[i];
                if (chars.IndexOf(c) > -1)
                {
                    isRemoved = true;
                }
                else
                {
                    sb.Append(c);
                }
            }

            var s = isRemoved ? sb.ToString() : value;
            FastStringBuilder.Free(sb);

            return s;
        }
        
        public static int ParseToInt32(this string value, int defaultValue = 0, int start = 0)
        {
            if (value == null || start >= value.Length)
            {
                return defaultValue;
            }
            var sign = 1;
            if (value[start] == '-')
            {
                sign = -1;
                ++start;
            }
            // int.MaxValue: 2147483647 -> 10digits
            uint result = 0;
            var i = 0;
            for (; i < 11; ++i)
            {
                var index = i + start;
                if (index >= value.Length) { break; }

                var c = value[index];
                int v;
                if (c >= '0' && c <= '9') { v = c - '0'; }
                else { break; }
                if (i == 11)
                {
                    return defaultValue;
                }
                result = result * 10 + (uint)v;
            }
            if (i == 0)
            {
                return defaultValue;
            }
            if (sign == 1 && result > (uint)int.MaxValue)
            {
                return defaultValue;
            }
            if (sign == -1 && result - 1 > (uint)(-(int.MinValue + 1)))
            {
                return defaultValue;
            }

            return unchecked((int)result) * sign;
        }
        
        public static int ParseHexToInt32(this string value, int defaultValue = 0)
        {
            var start = 0;
            if (value == null || start >= value.Length)
            {
                return defaultValue;
            }

            var sign = 1;
            if (value[start] == '-')
            {
                sign = -1;
                ++start;
            }
            uint result = 0;
            var i = 0;
            for (; i < 9; ++i)
            {
                if (i + start >= value.Length) { break; }
                var c = value[i + start];
                int v;
                if (c >= '0' && c <= '9') { v = c - '0'; }
                else if (c >= 'a' && c <= 'f') { v = c - 'a' + 10; }
                else if (c >= 'A' && c <= 'F') { v = c - 'A' + 10; }
                else { break; }
                if (i == 8)
                {
                    return defaultValue;
                }
                result |= (uint)v << (28 - i * 4);
            }
            if (i == 0)
            {
                return defaultValue;
            }
            if (i < 10)
            {
                result >>= 32 - i * 4;
            }

            var iValue = unchecked((int)result) * sign;
            return iValue;
        }
        
        public static string ToUpperFast(this string value)
        {
            if (value == null) return null;

            var len = value.Length;
            EnsureBufferSize(len);
            var buf = sharedBuffer;
            value.CopyTo(0, buf, 0, len);
            for (var i = 0; i < len; ++i)
            {
                var c = buf[i];
                if (c >= 'a' && c <= 'z')
                {
                    buf[i] = (char)(c - 'a' + 'A');
                }
            }
            var s = WeakStringPool.Shared.GetOrAdd(buf, 0, len);
            return s;
        }
        
        public static string ReplaceFast(this string value, char from, char to)
        {
            if (value == null) return null;
            if (value.IndexOf(from) == -1)
            {
                return value;
            }

            var len = value.Length;
            EnsureBufferSize(len);
            var buf = sharedBuffer;
            value.CopyTo(0, buf, 0, 0);
            for (var i = 0; i < len; ++i)
            {
                if (buf[i] == from)
                {
                    buf[i] = to;
                }
            }
            var s = WeakStringPool.Shared.GetOrAdd(buf, 0, len);
            return s;
        }

        #endregion 
        
        #region 
        public static int CompareToFast(this string s1, string s2)
        {
            // https://note.dokeep.jp/post/csharp-string-compare-options/
            return compareInfo.Compare(s1, s2, CompareOptions.Ordinal);
        }

        public class StringOrdinalComparer : IComparer<string>
        {
            public static readonly StringOrdinalComparer Shared = new StringOrdinalComparer();

            public int Compare(string x, string y)
            {
                return compareInfo.Compare(x, y, CompareOptions.Ordinal);
            }
        }
        #endregion

        #region 

        // 
        public const int SPLIT_COL_NUM = 8;
        
        static char[] sharedCharsForSplit;
        

        public static int Split( string str, char splitChar, ref string[] output )
        {
            if( output == null ) output = new string[ SPLIT_COL_NUM ];
            int count_of_delimiter = output.Length - 1;
            int count = 0;
            int start = 0;
            int i = 0;
            int max = str.Length;
            
            // 
            for( i = 0; i < max; ++i )
            {
                if( str[i] == splitChar )
                {
                    output[ count ] = str.Substring( start, i - start );
                    if( count == count_of_delimiter )
                    {
                        return count + 1;
                    }
                    
                    start = i + 1;
                    ++ count;
                }
            }
            
            output[ count ] = str.Substring( start, i - start );
            
            return count + 1;
        }
        
        public static string[] SplitBy(this string target, char separator)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            
            sharedCharsForSplit = sharedCharsForSplit ?? new char[1];
            sharedCharsForSplit[0] = separator;
            return target.Split(sharedCharsForSplit);
        }
        
        public static List<(int start, int length)> SplitRanges(this string target, char separator)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            var list = ListPool<(int, int)>.Rent();
            var start = 0;
            var pos = target.IndexOf(separator);
            while (pos > -1)
            {
                list.Add((start, pos - start));
                start = pos + 1;
                pos = target.IndexOf(separator, start);
            }

            list.Add((start, target.Length - start));
            return list;
        }
        
        public static string Substring(this string target, (int, int) tuple)
        {
            var offset = tuple.Item1;
            var count = tuple.Item2;
            EnsureBufferSize(count);
            var buf = sharedBuffer;
            for (var i = 0; i < count; ++i)
            {
                buf[i] = target[i + offset];
            }
            var s = WeakStringPool.Shared.GetOrAdd(buf, 0, count);
            sharedBuffer = buf;
            return s;
        }

        static void EnsureBufferSize(int minSize)
        {
            var buf = sharedBuffer ?? ArrayPool<char>.Shared.Rent(minSize);
            var capacity = buf.Length;
            if (capacity < minSize || capacity > (minSize << 3))
            {
                ArrayPool<char>.Shared.Return(buf);
                buf = ArrayPool<char>.Shared.Rent(minSize);
            }
            sharedBuffer = buf;
        }
        #endregion 

        #region 

        public static string GetFileNameOfPath(this string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            var i = path.LastIndexOfAny(PathSeperators);
            if (i == -1) return path;
            
            var start = i + 1;
            var count = path.Length - start;
            return path.Substring((start, count));
        }

        #endregion 

        #region 
        const int MaxFormatCacheCount = 256;
        
        public static string CachedFormat<T>(this string format, T arg)
        {
            if (FormatCache<T>.TryGet(format, arg, out var cache, out var result))
            {
                return result;
            }
            
            #pragma warning disable HAA0601
            result = string.Format(TextFormatProvider.Shared, format, arg);
            #pragma warning restore HAA0601
            if (cache.Count >= MaxFormatCacheCount)
            {
                foreach (var pair in cache)
                {
                    cache.Remove(pair.Key);
                    break;
                }
            }
            cache.Add(arg, result);

            return result;
        }


        public static string CachedFormat<T1, T2>(this string format, T1 arg1, T2 arg2)
        {
            var t = (arg1, arg2);
            if (FormatCache<(T1, T2)>.TryGet(format, t, out var cache, out var result))
            {
                return result;
            }

            #pragma warning disable HAA0601
            result = string.Format(TextFormatProvider.Shared, format, arg1, arg2);
            #pragma warning restore HAA0601
            if (cache.Count >= MaxFormatCacheCount)
            {
                foreach (var pair in cache)
                {
                    cache.Remove(pair.Key);
                    break;
                }
            }
            cache.Add(t, result);

            return result;
        }

  
        public static string CachedFormat<T1, T2, T3>(this string format, T1 arg1, T2 arg2, T3 arg3)
        {
            var t = (arg1, arg2, arg3);
            if (FormatCache<(T1, T2, T3)>.TryGet(format, t, out var cache, out var result))
            {
                return result;
            }

            #pragma warning disable HAA0601
            result = string.Format(TextFormatProvider.Shared, format, arg1, arg2, arg3);
            #pragma warning restore HAA0601
            if (cache.Count >= MaxFormatCacheCount)
            {
                foreach (var pair in cache)
                {
                    cache.Remove(pair.Key);
                    break;
                }
            }
            cache.Add(t, result);

            return result;
        }

        static class FormatCache<T>
        {
            static Dictionary<string, Dictionary<T, string>> formatCache;
            public static bool TryGet(string format, T arg, out Dictionary<T, string> cache, out string result)
            {
                if (formatCache == null)
                {
                    formatCache = new Dictionary<string, Dictionary<T, string>>();
                }

                if (!formatCache.TryGetValue(format, out cache))
                {
                    cache = new Dictionary<T, string>();
                    formatCache.Add(format, cache);
                }

                return cache.TryGetValue(arg, out result);
            }
        }

        // {:U}=ToUpperCase()
        // {:L}=ToLowerCase()
        // {:R,hello,world}=Replace("hello", "world")
        sealed class TextFormatProvider : IFormatProvider, ICustomFormatter
        {
            public static TextFormatProvider Shared = new TextFormatProvider();

            object IFormatProvider.GetFormat(Type formatType)
            {
                return formatType == typeof(ICustomFormatter) ? this : null;
            }

            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                if (arg == null)
                {
                    return "";
                }

                if (arg is string value)
                {
                    foreach (var fmt in format.SplitBy(';'))
                    {
                        switch (fmt)
                        {
                            case "u":
                            case "U":
                                value = CultureInfo.CurrentCulture.TextInfo.ToUpper(value);
                                continue;
                            case "l":
                            case "L":
                                value = CultureInfo.CurrentCulture.TextInfo.ToLower(value);
                                continue;
                            default: break;
                        }

                        if (fmt.StartsWith("R,"))
                        {
                            var i = fmt.IndexOf(',', 2);
                            var olds = fmt.Substring(2, i - 2);
                            var news = fmt.Substring(i + 1);
                            value = value.Replace(olds, news);
                            continue;
                        }

                        if (string.IsNullOrEmpty(fmt))
                        {
                            continue;
                        }

                        throw new FormatException("'" + format + "'は不正な書式指定子です");
                    }

                    return value;
                }

                if (arg is IFormattable f)
                {
                    return f.ToString(format, formatProvider);
                }

                return arg.ToString();
            }
        }
        #endregion 
    }
}
