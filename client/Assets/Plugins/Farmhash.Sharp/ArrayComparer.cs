using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
#pragma warning disable CA1062

namespace FastHashing
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    public static class ArrayComparer
    {
        // https://github.com/microsoft/BuildXL/blob/master/Public/Src/Cache/ContentStore/Hashing/ByteArrayComparer.cs
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe bool Equals32(byte* p1, byte* p2, int length)
        {
            if (p1 == null)
            {
                return p2 == null;
            }
            if (p2 == null)
            {
                return false;
            }

            {
                byte* x1 = p1, x2 = p2;
                int l = length;
                int n = l / 4;

                for (int i = 0; i < n; i++, x1 += 4, x2 += 4)
                {
                    if (*((int*)x1) != *((int*)x2))
                    {
                        return false;
                    }
                }

                if ((l & 2) != 0)
                {
                    if (*((short*)x1) != *((short*)x2))
                    {
                        return false;
                    }

                    x1 += 2; x2 += 2;
                }

                if ((l & 1) != 0)
                {
                    if (*x1 != *x2)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe bool Equals64(byte* p1, byte* p2, int length)
        {
            if (p1 == null)
            {
                return p2 == null;
            }
            if (p2 == null)
            {
                return false;
            }

            {
                byte* x1 = p1, x2 = p2;
                int l = length;
                int n = l / 8;
                for (int i = 0; i < n; i++, x1 += 8, x2 += 8)
                {
                    if (*((long*)x1) != *((long*)x2))
                    {
                        return false;
                    }
                }

                if ((l & 4) != 0)
                {
                    if (*((int*)x1) != *((int*)x2))
                    {
                        return false;
                    }

                    x1 += 4; x2 += 4;
                }

                if ((l & 2) != 0)
                {
                    if (*((short*)x1) != *((short*)x2))
                    {
                        return false;
                    }

                    x1 += 2; x2 += 2;
                }

                if ((l & 1) != 0)
                {
                    if (*x1 != *x2)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe bool Equals(byte* p1, byte* p2, int length)
        {
            if (IntPtr.Size == 4)
            {
                return Equals32(p1, p2, length);
            }
            else
            {
                return Equals64(p1, p2, length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe bool CheckArguments<T>(T[] x, int xOffset, int xLength, T[] y, int yOffset, int yLength, out bool result)
        {
            if (x == null)
            {
                result = y == null;
                return true;
            }
            if (y == null || xLength != yLength)
            {
                result = false;
                return true;
            }
            if (xLength == 0)
            {
                result = true;
                return true;
            }

            var xLen = x.Length;
            var yLen = y.Length;
            if ((uint)xOffset > (uint)xLen) throw new ArgumentOutOfRangeException(nameof(xOffset));
            if ((uint)yOffset > (uint)yLen) throw new ArgumentOutOfRangeException(nameof(xOffset));
            if ((uint)xLength > (uint)xLen) throw new ArgumentOutOfRangeException(nameof(xOffset));
            if ((uint)yLength > (uint)yLen) throw new ArgumentOutOfRangeException(nameof(xOffset));
            if (xLen < xOffset + xLength) throw new ArgumentOutOfRangeException(nameof(x));
            if (yLen < yOffset + yLength) throw new ArgumentOutOfRangeException(nameof(y));

            result = false;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Equals(char[] x, int xOffset, int xLength, char[] y, int yOffset, int yLength)
        {
            if (CheckArguments(x, xOffset, xLength, y, yOffset, yLength, out var result))
            {
                return result;
            }

            fixed (char* p1 = &x[xOffset], p2 = &y[yOffset])
            {
                return Equals((byte*)p1, (byte*)p2, xLength * sizeof(char));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SafeEquals(char[] x, int xOffset, int xLength, char[] y, int yOffset, int yLength)
        {
            if (CheckArguments(x, xOffset, xLength, y, yOffset, yLength, out var result))
            {
                return result;
            }

            for (var i = 0; i < xLength; ++i)
            {
                if (x[i + xOffset] != y[i + yOffset])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
