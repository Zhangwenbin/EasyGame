using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FastHashing;
using Unity.IL2CPP.CompilerServices;

namespace EG
{
    //=========================================================================
    //. ArraySegment<char>の拡張
    //=========================================================================
    public static class CharArraySegmentExtensions
    {
        static char[] sharedCharArray = new char[16];

        /// <summary>ArraySegmentを作る.</summary>
        public static ArraySegment<char> SharedSegment(this string s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (sharedCharArray.Length < s.Length)
            {
                ArrayPool<char>.Shared.Return(sharedCharArray);
                sharedCharArray = ArrayPool<char>.Shared.Rent(s.Length);
            }
            s.CopyTo(0, sharedCharArray, 0, s.Length);
            return new ArraySegment<char>(sharedCharArray, 0, s.Length);
        }

        /// <summary>ArraySegmentを作る.</summary>
        public static ArraySegment<char> Segment(this string s, int start, int count, bool allocates = false)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if ((uint)count > (uint)s.Length) throw new ArgumentOutOfRangeException(nameof(count));
            if ((uint)start > (uint)s.Length) throw new ArgumentOutOfRangeException(nameof(start));
            if (start + count > s.Length) throw new ArgumentOutOfRangeException(nameof(start));

            var array = count == 0 ? Array.Empty<char>() : allocates ? new char[count + 2] : ArrayPool<char>.Shared.Rent(count);
            s.CopyTo(start, array, 0, count);
            return new ArraySegment<char>(array, 0, count);
        }

        /// <summary>ArraySegmentを作る.</summary>
        public static ArraySegment<char> Segment(this string s, int start, bool allocates = false)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if ((uint)start > (uint)s.Length) throw new ArgumentOutOfRangeException(nameof(start));
            return Segment(s, start, s.Length - start, allocates);
        }

        /// <summary>ArraySegmentを作る.</summary>
        public static ArraySegment<char> Segment(this string s, bool allocates = false)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            return Segment(s, 0, s.Length, allocates);
        }

        /// <summary>ArraySegmentを作る.切り出すだけ.</summary>
        public static ArraySegment<char> Segment(this char[] array, int offset, int count)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            return new ArraySegment<char>(array, offset, count);
        }

        /// <summary>Arrayのサイズを最低限に.</summary>
        public static ArraySegment<char> Compact(this ArraySegment<char> seg)
        {
            return Clone(seg, true);
        }

        /// <summary>複製を作る. Arrayも複製される.</summary>
        public static ArraySegment<char> Clone(this ArraySegment<char> seg, bool allocates = false)
        {
            var count = seg.Count;
            var array = count == 0 ? Array.Empty<char>() : allocates ? new char[count + 2] : ArrayPool<char>.Shared.Rent(count);
            // BlockCopyはbyte単位指定なのでsizeof(char)を掛ける
            if (count > 0)
            {
                Buffer.BlockCopy(seg.Array, seg.Offset * 2, array, 0, count * 2);
            }
            return new ArraySegment<char>(array, 0, count);
        }

        public static string GetString(this ArraySegment<char> seg)
        {
            if (IsNullOrEmpty(seg)) return "";
            return new string(seg.Array, seg.Offset, seg.Count);
        }

        public static string PopString(ref this ArraySegment<char> seg)
        {
            var s = GetString(seg);
            Free(ref seg);
            return s;
        }

        public static bool IsNullOrEmpty(this ArraySegment<char> seg)
        {
            return seg.Array == null || seg.Count == 0;
        }

        public static void Free(ref this ArraySegment<char> seg)
        {
            var array = seg.Array;
            if (array == null || array.Length == 0)
            {
                return;
            }
            seg = CharArraySegment.Empty;

            if (ReferenceEquals(array, sharedCharArray))
            {
                return;
            }
            ArrayPool<char>.Shared.Return(array);
        }

        public static bool AreEqual(this ArraySegment<char> x, ArraySegment<char> y)
        {
            return CharArraySegmentEqualityComparer.AreEqual(x, y);
        }

        public static int LastIndexOf(ref this ArraySegment<char> seg, char needle, int startIndex = -1)
        {
            var array = seg.Array;
            var offset = seg.Offset;
            var count = seg.Count;
            if (startIndex < 0)
                startIndex = count + startIndex;
            if ((uint)startIndex >= (uint)count)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            for (var i = startIndex; i >= 0; --i)
            {
                if (array[offset + i] == needle)
                    return i;
            }
            return -1;
        }

        public static Dictionary<ArraySegment<char>, T> ToCharArraySegmentKeyDictionary<T>(this Dictionary<string, T> source, Dictionary<ArraySegment<char>, T> dest = null)
        {
            if (dest != null)
            {
                dest.Clear();
            }
            var dict = dest ?? new Dictionary<ArraySegment<char>, T>(source.Count, CharArraySegmentEqualityComparer.Shared);

            foreach (var (k, v) in source)
            {
                dict[k.Segment(true)] = v;
            }
            return dict;
        }

        #region Format

        /// <summary>
        /// ArraySegment&lt;char&gt;で返す.
        /// rentsArray==falseで共有char[]を使われるためFree不要だが、
        /// 次回のコールで書き換えられるので保持してはいけない
        /// </summary>
        public static ArraySegment<char> FormatToCharArray(this int value, bool rentsArray = false)
        {
            // int.MinValue -> -2147483648 -> 11chars
            var array = rentsArray ? ArrayPool<char>.Shared.Rent(16) : sharedCharArray;
            var sb = FastStringBuilder.Alloc();
            sb.Append(value);
            sb.WriteTo(array);
            var seg = new ArraySegment<char>(array, 0, sb.Length);
            FastStringBuilder.Free(sb);
            return seg;
        }

        /// <summary>
        /// アロケーションがおきないかもしれない
        /// </summary>
        public static string FormatToString(this int value)
        {
            var sb = FastStringBuilder.Alloc();
            sb.Append(value);
            var s = sb.ToString();
            FastStringBuilder.Free(sb);
            return s;
        }
        /// <summary>
        /// アロケーションがおきないかもしれない
        /// </summary>
        public static string FormatToString(this uint value)
        {
            var sb = FastStringBuilder.Alloc();
            sb.Append(value);
            var s = sb.ToString();
            FastStringBuilder.Free(sb);
            return s;
        }
        /// <summary>
        /// アロケーションがおきないかもしれない
        /// </summary>
        public static string FormatToString(this long value)
        {
            var sb = FastStringBuilder.Alloc();
            sb.Append(value);
            var s = sb.ToString();
            FastStringBuilder.Free(sb);
            return s;
        }
        #endregion
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    public class CharArraySegmentEqualityComparer : IEqualityComparer<ArraySegment<char>>
    {
        public static readonly CharArraySegmentEqualityComparer Shared = new CharArraySegmentEqualityComparer();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreEqual(ArraySegment<char> x, ArraySegment<char> y)
        {
            // 名前がEqualsじゃないのはobject.Equalsを避けるため
            if (x.Count != y.Count) return false;
            if (x.Array == null && y.Array == null && x.Count == 0) return true;
            if (x.Array == null || y.Array == null) return false;

            return ArrayComparer.SafeEquals(x.Array, x.Offset, x.Count, y.Array, y.Offset, y.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ArraySegment<char> x, ArraySegment<char> y)
        {
            return AreEqual(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(ArraySegment<char> seg)
        {
            if (seg.Array == null)
            {
                return 0;
            }
            return FNVHash.Hash32(seg.Array, seg.Offset, seg.Count);
        }
    }

    public static class CharArraySegment
    {
        public static readonly ArraySegment<char> Empty = new ArraySegment<char>(Array.Empty<char>(), 0, 0);
    }
}
