/**************************************************************************/
/*! @file   WeakStringPool.cs
***************************************************************************/
 #define USED_WEAKSTRINGPOOL
// #define PROFILING
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
    #if USED_WEAKSTRINGPOOL
    public class WeakStringPool
    {
        public static readonly WeakStringPool Shared = new WeakStringPool();

        public static readonly int[] bucketSizes = {
            7, 13, 31, 61, 127, 251, 509, 1021, 2039, 4093, 8191, 16381,
            32749, 65521, 131071, 262139, 524287, 1048573, 2097143,
            4194301, 8388593, 16777213, 33554393, 67108859, 134217689,
            268435399, 536870909, 1073741789};
        const int BucketLoadFactor = 55;
        const int CollisionLoadFactor = 35;
        const int AliveCheckFrequency = 10;

        WeakReference<string>[] values;
        long[] buckets;
        int[] free;
        WeakReference<string>[] pooledWeak;
        int bucketSize;
        int valueEnd;
        int freeCount;
        int pooledWeakCount;
        int aliveCheck;
        int collision;

        #if PROFILING
        double searchCount;
        double hitCount;
        public double CollisionRate => (double)collision / valueEnd;
        public double HitRate => hitCount / searchCount;
        #endif

        public WeakStringPool()
        {
            valueEnd = 0;
            pooledWeakCount = 0;
            bucketSize = 7;
            aliveCheck = AliveCheckFrequency;
            buckets = ArrayPool<long>.Shared.Rent(8);
            values = ArrayPool<WeakReference<string>>.Shared.Rent(4);
            pooledWeak = ArrayPool<WeakReference<string>>.Shared.Rent(4);
            free = ArrayPool<int>.Shared.Rent(4);
        }

        /// <summary>
        /// 渡されたchar[]の値から作られたstringが既に作成済みで
        /// gcに回収されていなかったら、新規作成せずそのstringを返す.
        /// </summary>
        public string GetOrAdd(char[] array, int offset, int count)
        {
            if (count == 0) return "";
            var key = new ArraySegment<char>(array, offset, count);

            if (100 * (valueEnd-freeCount) >= BucketLoadFactor * bucketSize)
            {
                Rehash();
            }

            #if PROFILING
            searchCount += 1.0;
            #endif

            var hash = ComputeHash(array, offset, count);
            var i = FindEntryIndexOrEmptyBucket(
                buckets, bucketSize, hash, key, out var target, out var collis);
            if (i > -1)
            {
                if (target == null)
                {
                    Throw<InvalidProgramException>("invalid weakreference");
                }

                #if PROFILING
                hitCount += 1.0;
                #endif

                if (--aliveCheck < 0)
                {
                    aliveCheck = AliveCheckFrequency;
                    var j = (int)((uint)hash % valueEnd);  // random
                    if (j != i) CheckFreeValueEntry(j);
                }
                return target;
            }

            var bucketIndex = ~i;
            var s = new string(key.Array, key.Offset, key.Count);
            AddEntry(bucketIndex, hash, s);
            collision += collis;

            return s;
        }

        public string GetOrAdd(ArraySegment<char> key)
        {
            return GetOrAdd(key.Array, key.Offset, key.Count);
        }

        void AddEntry(int bucketIndex, int hash, string s)
        {
            WeakReference<string> weak;
            if (pooledWeakCount > 0)
            {
                weak = pooledWeak[--pooledWeakCount];
                weak.SetTarget(s);
            }
            else
            {
                weak = new WeakReference<string>(s);
            }
            int entryIndex;
            if (freeCount > 0)
            {
                entryIndex = free[--freeCount];
            }
            else
            {
                entryIndex = valueEnd;
                if (values.Length < entryIndex + 1)
                {
                    ResizeArray(ref values, entryIndex, entryIndex + 1);
                }
                ++valueEnd;
            }
            values[entryIndex] = weak;
            buckets[bucketIndex] = ((long)entryIndex + 1) | ((long)hash << 32);
        }

        static void ResizeArray<T>(ref T[] array, int oldSize, int minSize)
        {
            if (array.Length >= minSize)
            {
                return;
            }
            var oldArray = array;
            var newArray = ArrayPool<T>.Shared.Rent(NextPOT(minSize));
            if (oldSize > 0)
            {
                Array.Copy(oldArray, 0, newArray, 0, oldSize);
            }
            array = newArray;
            ArrayPool<T>.Shared.Return(oldArray);
        }

        static int NextPOT(int n)
        {
            if (n < 4) return 4;
            --n;
            while ((n & (n - 1)) != 0)
            {
                n &= n - 1;
            }
            return n << 1;
        }

        static int NextBucketSize(int n)
        {
            foreach (var size in bucketSizes)
            {
                if (size >= n)
                {
                    return size;
                }
            }
            return n;
        }

        int FindEntryIndexOrEmptyBucket(
            long[] buckets, int bucketSize, int hash,
            ArraySegment<char> key, out string target, out int collision)
        {
            var i = (int)((uint)hash % (uint)bucketSize);
            var firstFree = -1;
            var lastFree = -1;
            WeakReference<string> weak = null;
            collision = 0;

            while (true)
            {
                var e = buckets[i];
                var entry = (int)e;
                if (entry == 0)
                {
                    // empty bucket
                    target = null;
                    if (lastFree != -1) buckets[lastFree] = 0;
                    if (firstFree != -1) return ~firstFree;
                    return ~i;
                }

                if (entry < 0)
                {
                    // free bucket
                    if (firstFree == -1) firstFree = i;
                    lastFree = i;
                    goto Next;
                }

                --entry;
                var eh = (int)(e >> 32);
                if (eh != hash)
                {
                    // unmatched hash
                    weak = values[entry];
                    if (weak == null)
                    {
                        // free
                        buckets[entry] = -1;
                        if (firstFree == -1) firstFree = i;
                        lastFree = i;
                    }
                    else
                    {
                        if (--aliveCheck < 0)
                        {
                            aliveCheck = AliveCheckFrequency;
                            if (!weak.TryGetTarget(out _)) // slow..
                            {
                                goto Free;
                            }
                        }
                        // other value
                        collision++;
                        lastFree = -1;
                    }
                    goto Next;
                }

                weak = values[entry];
                if (weak == null)
                {
                    buckets[entry] = -1;
                    if (firstFree == -1) firstFree = i;
                    lastFree = i;
                    goto Next;
                }

                if (weak.TryGetTarget(out target))
                {
                    if (EqualsToString(key, target))
                    {
                        if (firstFree != -1)
                        {
                            buckets[firstFree] = e;
                            buckets[i] = 0;
                        }
                        return entry;
                    }
                    collision++;
                    goto Next;
                }

                Free:
                // target was gc
                FreeValueEntry(entry, weak);
                buckets[i] = -1;
                if (firstFree == -1) firstFree = i;
                lastFree = i;

                Next:
                ++i;
                if (i >= bucketSize) i = 0;
            }
        }

        void CheckFreeValueEntry(int valueIndex)
        {
            var w = values[valueIndex];
            if (w?.TryGetTarget(out _) == false)
            {
                FreeValueEntry(valueIndex, w);
            }
        }

        void FreeValueEntry(int valueIndex, WeakReference<string> weak)
        {
            // var weak = values[valueIndex];
            if (weak == null) return;

            if (free.Length < freeCount + 1)
            {
                ResizeArray(ref free, freeCount, freeCount + 1);
            }
            if (pooledWeak.Length < pooledWeakCount + 1)
            {
                ResizeArray(ref pooledWeak, pooledWeakCount, pooledWeakCount + 1);
            }
            weak.SetTarget(null);
            free[freeCount++] = valueIndex;
            pooledWeak[pooledWeakCount++] = weak;
            values[valueIndex] = null;
        }

        static bool EqualsToString(ArraySegment<char> a, string b)
        {
            var count = a.Count;
            if (count != b.Length) return false;
            var array = a.Array;
            var i = a.Offset;
            var j = 0;
            var end = i + count;

            while (i < end)
            {
                if (array[i++] != b[j++]) return false;
            }
            return true;
        }

        public static int ComputeHash(char[] array, int offset, int count)
        {
            var i = offset;
            var end = i + count;

            var h = 2166136261U;
            while (i < end)
            {
                h = (h * 16777619U) ^ array[i++];
            }
            return (int)h;
        }

        static readonly object[] throwParams = new object[1];

        static void Throw<T>(string key)
        {
            throwParams[0] = key;
            throw (Exception)Activator.CreateInstance(typeof(T), throwParams);
        }

        void Rehash()
        {
            for (var i = 0; i < valueEnd; ++i)
            {
                CheckFreeValueEntry(i);
            }

            if ( (100 * (valueEnd-freeCount) >= BucketLoadFactor * bucketSize) == false )
            {
                return;
            }

            var oldSize = bucketSize;
            var oldBuckets = buckets;
            var newSize = NextBucketSize(bucketSize + 1);
            var newBuckets = ArrayPool<long>.Shared.Rent(newSize);
            Array.Clear(newBuckets, 0, newSize);

            var empty = new ArraySegment<char>(Array.Empty<char>(), 0, 0);
            var entryCount = valueEnd;
            aliveCheck = int.MaxValue;
            collision = 0;
            for (var i = 0; i < oldSize && entryCount > 0; ++i)
            {
                var bucket = oldBuckets[i];
                var entryNumber = (int)bucket;
                if (entryNumber < 0)
                {
                    entryCount--;
                }
                else if (entryNumber > 0 && values[entryNumber - 1] == null)
                {
                    entryCount--;
                    entryNumber = -1;
                }
                if (entryNumber <= 0) continue;

                var hash = (int)(bucket >> 32);
                var k = FindEntryIndexOrEmptyBucket(newBuckets, newSize, hash, empty, out _, out var collis);
                if (k > -1) Throw<InvalidProgramException>("duplicate in rehash");  // duplicate?
                newBuckets[~k] = bucket;
                entryCount -= 1;
                collision += collis;
            }

            buckets = newBuckets;
            bucketSize = newSize;
            ArrayPool<long>.Shared.Return(oldBuckets);
            aliveCheck = AliveCheckFrequency;
        }
    }
    #else
    public class WeakStringPool
    {
        public static readonly WeakStringPool Shared = new WeakStringPool();
        
        public string GetOrAdd(char[] array, int offset, int count)
        {
            return new string( array, offset, count );
        }
    }
    #endif
}
