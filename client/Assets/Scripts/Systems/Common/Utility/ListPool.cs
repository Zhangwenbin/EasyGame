using System.Collections.Generic;
using System;

namespace EG
{
    public static class ListPool<T>
    {
        #pragma warning disable CA1000
     
        public static List<T> Rent()
        {
            var list = InstancePool<List<T>>.Shared.Rent();
            if (list != null)
            {
                list.Clear();
                return list;
            }

            return new List<T>();
        }


        public static void Return(List<T> list)
        {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }

            list.Clear();
            InstancePool<List<T>>.Shared.Return(list);
        }
        #pragma warning restore CA1000
    }

    public static class ListPool
    {
        static List<T> Rent<T>()
        {
            var list = InstancePool<List<T>>.Shared.Rent();
            if (list != null)
            {
                list.Clear();
                return list;
            }

            return new List<T>();
        }

        public static void Return<T>(ref List<T> list)
        {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }

            list.Clear();
            InstancePool<List<T>>.Shared.Return(list);
            list = null;
        }

        public static void Free<T>(this List<T> list)
        {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }

            list.Clear();
            InstancePool<List<T>>.Shared.Return(list);
        }

        public static void Dispose<T>(List<T> list)
            where T : class, IDisposable
        {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }
            
            for (var i = 0; i < list.Count; ++i)
            {
                var item = list[i];
                if (item != null)
                {
                    item.Dispose();
                    list[i] = null;
                }
            }
            list.Clear();
            InstancePool<List<T>>.Shared.Return(list);
            list = null;
        }

        /// 
        public static List<T> RentList<T>(this List<T> list)
        {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }

            var result = Rent<T>();
            if (result.Capacity < list.Count)
            {
                result.Capacity = list.Count;
            }
            foreach (var item in list)
            {
                result.Add(item);
            }
            return result;
        }

        /// 
        public static List<T> RentList<T>(this T[] array)
        {
            if (array == null) { throw new ArgumentNullException(nameof(array)); }

            var result = Rent<T>();
            if (result.Capacity < array.Length)
            {
                result.Capacity = array.Length;
            }
            foreach (var item in array)
            {
                result.Add(item);
            }
            return result;
        }

        public static List<T> Filter<T>(this List<T> list, Func<T, bool> predecate)
        {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }

            var result = Rent<T>();
            foreach (var item in list)
            {
                if (predecate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static List<T> Filter<T>(this T[] array, Func<T, bool> predecate)
        {
            if (array == null) { throw new ArgumentNullException(nameof(array)); }

            var result = Rent<T>();
            for (var i = 0; i < array.Length; ++i)
            {
                var item = array[i];
                if (predecate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static List<T> Filter<T, TContext>(this List<T> list, Func<T, TContext, bool> predecate, TContext context)
        {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }

            var result = Rent<T>();
            foreach (var item in list)
            {
                if (predecate(item, context))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static List<T> FilterEquals<T, TKey>(this List<T> list, Func<T, TKey> func, TKey key)
        {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }

            var comparer = EqualityComparer<TKey>.Default;
            var result = Rent<T>();
            foreach (var item in list)
            {
                if (comparer.Equals(func(item), key))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static List<TDest> Map<TSource, TDest>(this List<TSource> sourceList, Func<TSource, TDest> map)
        {
            var list = Rent<TDest>();
            if (list.Capacity < sourceList.Count)
            {
                list.Capacity = sourceList.Count;
            }
            foreach (var item in sourceList)
            {
                list.Add(map(item));
            }
            return list;
        }

        public static List<TDest> Map<TSource, TDest>(this TSource[] sourceList, Func<TSource, TDest> map)
        {
            if (sourceList == null) { throw new ArgumentNullException(nameof(sourceList)); }

            var list = Rent<TDest>();
            if (list.Capacity < sourceList.Length)
            {
                list.Capacity = sourceList.Length;
            }
            foreach (var item in sourceList)
            {
                list.Add(map(item));
            }
            return list;
        }

        public static List<TDest> Map<TSource, TDest, TContext>(this TSource[] sourceList, Func<TSource, TContext, TDest> map, TContext context)
        {
            if (sourceList == null) { throw new ArgumentNullException(nameof(sourceList)); }

            var list = Rent<TDest>();
            if (list.Capacity < sourceList.Length)
            {
                list.Capacity = sourceList.Length;
            }
            foreach (var item in sourceList)
            {
                list.Add(map(item, context));
            }
            return list;
        }

        /// 
        public static List<(T1, T2)> Zip<T1, T2>(this List<T1> list, IList<T2> otherList)
        {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }
            if (otherList == null) { throw new ArgumentNullException(nameof(otherList)); }

            var result = ListPool<(T1, T2)>.Rent();
            for (var i = 0; i < list.Count; ++i)
            {
                if (i >= otherList.Count) break;
                result.Add((list[i], otherList[i]));
            }
            return result;
        }
    }
}
