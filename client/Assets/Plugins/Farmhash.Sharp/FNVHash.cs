using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace FastHashing
{
    // unsafeを一切使わない実装
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    public static class FNVHash
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckArguments(char[] s, int offset, int length)
        {
            if (s == null) throw new System.ArgumentNullException(nameof(s));
            if ((uint)length > (uint)s.Length) throw new System.ArgumentOutOfRangeException(nameof(length));
            if ((uint)offset > (uint)s.Length) throw new System.ArgumentOutOfRangeException(nameof(offset));
            if (length + offset > s.Length) throw new System.ArgumentOutOfRangeException(nameof(length));
        }
        
        const uint FNV_OFFSET_BASIS_32 = 2166136261U;
        const uint FNV_PRIME_32 = 16777619U;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Hash32(char[] s, int offset, int length)
        {
            CheckArguments(s, offset, length);

            var end = offset + length;
            var hash = FNV_OFFSET_BASIS_32;

            for (var i = offset; i < end; ++i)
            {
                var c = (uint)s[i];
                hash = (FNV_PRIME_32 * hash) ^ (c & 0xff);
                hash = (FNV_PRIME_32 * hash) ^ (c >> 8);
            }

            return unchecked((int)hash);
        }
    }
}
