using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CSharpHelpers
{
    /// <summary>Enum</summary>
    public static class EnumSupport
    {
        public static int ToInt32<T>(this T value)
#if CSHARP_7_OR_LATER
            where T : struct, Enum
#else
            where T : struct, IComparable, IConvertible, IFormattable
#endif
        {
            return EnumSupport<T>.ToInt32(value);
        }

        public static long ToInt64<T>(this T value)
#if CSHARP_7_OR_LATER
            where T : struct, Enum
#else
            where T : struct, IComparable, IConvertible, IFormattable
#endif
        {
            return EnumSupport<T>.ToInt64(value);
        }

        public static T ToValue<T>(long i)
#if CSHARP_7_OR_LATER
            where T : struct, Enum
#else
            where T : struct, IComparable, IConvertible, IFormattable
#endif
        {
            return EnumSupport<T>.ToValue(i);
        }

        public static string GetName<T>(this T value)
#if CSHARP_7_OR_LATER
            where T : struct, Enum
#else
            where T : struct, IComparable, IConvertible, IFormattable
#endif
        {
            return EnumSupport<T>.GetName(value);
        }

        public static T Parse<T>(string name)
#if CSHARP_7_OR_LATER
            where T : struct, Enum
#else
            where T : struct, IComparable, IConvertible, IFormattable
#endif
        {
            T value;
            if (EnumSupport<T>.TryParse(name, out value))
            {
                return value;
            }

            throw new InvalidOperationException();
        }

        public static T? TryParse<T>(string name)
#if CSHARP_7_OR_LATER
            where T : struct, Enum
#else
            where T : struct, IComparable, IConvertible, IFormattable
#endif
        {
            T value;
            if (EnumSupport<T>.TryParse(name, out value))
            {
                return value;
            }

            return (T?)null;
        }

        public static bool TryParse<T>(string name, out T value)
#if CSHARP_7_OR_LATER
            where T : struct, Enum
#else
            where T : struct, IComparable, IConvertible, IFormattable
#endif
        {
            return EnumSupport<T>.TryParse(name, out value);
        }

        public static TAttr GetAttribute<T, TAttr>(this T value, TAttr defaultAttribute)
#if CSHARP_7_OR_LATER
            where T : struct, Enum
#else
            where T : struct, IComparable, IConvertible, IFormattable
#endif
            where TAttr : Attribute
        {
            return EnumSupport<T>.GetAttribute<TAttr>(value) ?? defaultAttribute;
        }

        public static T[] GetValues<T>()
#if CSHARP_7_OR_LATER
            where T : struct, Enum
#else
            where T : struct, IComparable, IConvertible, IFormattable
#endif
        {
            return EnumSupport<T>.GetValues();
        }

        public static string[] GetNames<T>()
#if CSHARP_7_OR_LATER
            where T : struct, Enum
#else
            where T : struct, IComparable, IConvertible, IFormattable
#endif
        {
            return EnumSupport<T>.GetNames();
        }

        /// <summary>Dictionaryのコンストラクタに渡して使う</summary>
        public static IEqualityComparer<T> GetEqualityComparer<T>()
#if CSHARP_7_OR_LATER
            where T : struct, Enum
#else
            where T : struct, IComparable, IConvertible, IFormattable
#endif
        {
            return EnumSupport<T>.EqualityComparer;
        }

        /// <summary>boxingの発生しないenumキーのDictionary作る</summary>
        public static Dictionary<T, TValue> CreateDictionary<T, TValue>()
#if CSHARP_7_OR_LATER
            where T : struct, Enum
#else
            where T : struct, IComparable, IConvertible, IFormattable
#endif
        {
            return new Dictionary<T, TValue>(EnumSupport<T>.EqualityComparer);
        }
    }

    /// <summary>Enumの補助(本実装・Type指定のコードが長くなるのでこっちは内部でのみ使う)</summary>
    internal static class EnumSupport<T>
#if CSHARP_7_OR_LATER
        where T : struct, Enum
#else
        where T : struct, IComparable, IConvertible, IFormattable
#endif
    {
        // static member initializer はこのクラスでは一切使わないように.

        // dictionaryを作るのはとても重いので、この回数までは、GC発生するけれどもEnum.Parse,ToStringを使う
        const int ParseCountTherethold = 4;

        static Type underlyingType;
        public static Type UnderlyingType { get { return underlyingType ?? (underlyingType = typeof(T).GetEnumUnderlyingType()); } }

        static EnumEqualityComparer<T> equalityComparer;
        public static IEqualityComparer<T> EqualityComparer { get { return equalityComparer ?? (equalityComparer = new EnumEqualityComparer<T>()); } }

        static T[] values;
        static string[] names;
        static Dictionary<string, long> valueByName;
        static Dictionary<long, string> nameByValue;
        static Dictionary<long, FieldInfo> fieldByValue;
        static int parseCount;

        static class AttributeCache<TAttr> where TAttr : Attribute
        {
            public static Dictionary<long, TAttr> attrByValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long ToInt64(T x)
        {
            var ut = UnderlyingType;
            if (ut == typeof(byte) || ut == typeof(sbyte))
            {
                return (long)ValueCast<T, byte>.Convert(x);
            }
            else if (ut == typeof(short) || ut == typeof(ushort))
            {
                return (long)ValueCast<T, short>.Convert(x);
            }
            else if (ut == typeof(int) || ut == typeof(uint))
            {
                return (long)ValueCast<T, int>.Convert(x);
            }
            else if (ut == typeof(long) || ut == typeof(ulong))
            {
                return ValueCast<T, long>.Convert(x);
            }

            throw new ArgumentException("unknown UnderlyingType: " + ut);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int ToInt32(T x)
        {
            var ut = UnderlyingType;
            if (ut == typeof(byte) || ut == typeof(sbyte))
            {
                return (int)ValueCast<T, byte>.Convert(x);
            }
            else if (ut == typeof(short) || ut == typeof(ushort))
            {
                return (int)ValueCast<T, short>.Convert(x);
            }
            else if (ut == typeof(int) || ut == typeof(uint))
            {
                return ValueCast<T, int>.Convert(x);
            }
            else if (ut == typeof(long) || ut == typeof(ulong))
            {
                throw new ArgumentException("UnderlyingType is long");
            }

            throw new ArgumentException("unknown UnderlyingType: " + ut);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T ToValue(long value)
        {
            var ut = UnderlyingType;
            if (ut == typeof(byte) || ut == typeof(sbyte))
            {
                return ValueCast<byte, T>.Convert((byte)value);
            }
            else if (ut == typeof(short) || ut == typeof(ushort))
            {
                return ValueCast<short, T>.Convert((short)value);
            }
            else if (ut == typeof(int) || ut == typeof(uint))
            {
                return ValueCast<int, T>.Convert((int)value);
            }
            else if (ut == typeof(long) || ut == typeof(ulong))
            {
                return ValueCast<long, T>.Convert(value);
            }

            throw new ArgumentException("unknown UnderlyingType: " + ut);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T[] GetValues()
        {
            return values ?? (values = (T[])Enum.GetValues(typeof(T)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string[] GetNames()
        {
            return names ?? (names = (string[])Enum.GetNames(typeof(T)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(T t)
        {
            if (nameByValue == null && ++parseCount > ParseCountTherethold)
            {
                BuildDictionary();
            }

            if (nameByValue == null)
            {
                return t.ToString();
            }

            string name;
            if (nameByValue.TryGetValue(ToInt64(t), out name))
            {
                return name;
            }

            return t.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TryParse(string name, out T value)
        {
            if (valueByName == null && ++parseCount > ParseCountTherethold)
            {
                BuildDictionary();
            }

            if (valueByName == null)
            {
                return Enum.TryParse(name, out value);
            }

            if (valueByName.TryGetValue(name, out var i))
            {
                value = ToValue(i);
                return true;
            }

            value = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void BuildDictionary()
        {
            var values = GetValues();

            valueByName = new Dictionary<string, long>(values.Length);
            nameByValue = new Dictionary<long, string>(values.Length);

            foreach (var value in values)
            {
                #pragma warning disable HAA0102
                var name = value.ToString();
                #pragma warning restore HAA0102
                var i = ToInt64(value);
                valueByName[name] = i;
                nameByValue[i] = name;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static FieldInfo GetField(T value)
        {
            if (fieldByValue == null)
            {
                fieldByValue = new Dictionary<long, FieldInfo>();
            }

            var i = ToInt64(value);
            if (fieldByValue.TryGetValue(i, out var field))
            {
                return field;
            }

            field = typeof(T).GetField(GetName(value));
            fieldByValue.Add(i, field);
            return field;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static TAttr GetAttribute<TAttr>(T value)
            where TAttr : Attribute
        {
            var i = ToInt64(value);
            var attrByValue = AttributeCache<TAttr>.attrByValue;
            if (attrByValue == null)
            {
                attrByValue = new Dictionary<long, TAttr>();
                AttributeCache<TAttr>.attrByValue = attrByValue;
            }

            if (attrByValue.TryGetValue(i, out var attr))
            {
                return attr;
            }

            var field = GetField(value);
            attr = (TAttr)field.GetCustomAttribute(typeof(TAttr));
            attrByValue.Add(i, attr);  // can be null
            return attr;
        }
    }

    sealed class EnumEqualityComparer<T> : IEqualityComparer<T>
#if CSHARP_7_OR_LATER
        where T : struct, Enum
#else
        where T : struct, IComparable, IConvertible, IFormattable
#endif
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(T x, T y)
        {
            return EnumSupport<T>.ToInt64(x) == EnumSupport<T>.ToInt64(y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(T x)
        {
            var h = (uint)EnumSupport<T>.ToInt32(x);
            // FMix
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;
            return (int)h;
        }
    }

#if true
    static class ValueCast<TFrom, TTo>
        where TFrom : struct
        where TTo : struct
    {
        // [StructLayout(LayoutKind.Explicit)]
        // struct Reinterpret
        // {
        //     // GenericTypeにFieldOffset()つけるのはnet20でしか動かない
        //     [FieldOffset(0)] public TFrom From;
        //     [FieldOffset(0)] public TTo To;
        // }

        // public static TTo Convert(TFrom value)
        // {
        //     var r = default(Reinterpret);
        //     r.From = value;
        //     return r.To;
        // }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TTo Convert(TFrom value)
        {
            var result = default(TTo);
            unsafe
            {
                var ptr = Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AddressOf(ref result);
                Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr(ref value, ptr);
            }
            return result;
        }
    }
#else
    static class ValueCast<TFrom, TTo>
    {
        public static TTo Convert(TFrom value)
        {
            // needs https://www.nuget.org/packages/System.Runtime.CompilerServices.Unsafe/
            return System.Runtime.CompilerServices.ValueCast<TFrom, TTo>.Convert(ref value);
        }
    }
#endif
}
