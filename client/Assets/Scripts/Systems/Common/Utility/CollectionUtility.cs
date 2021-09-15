/**************************************************************************/
/*! @file   CollectionUtility.cs
    @brief  辞書ユーティリティ
***************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace EG
{

    public static class CollectionUtility
    {
        /// ***********************************************************************
        /// <summary>
        /// 範囲外ならnullを返す
        /// </summary>
        /// ***********************************************************************
        public static T GetOrNull<T>( this List<T> list, int index ) where T : class
        {
            if (list == null || (uint)index >= (uint)list.Count) return null;
            return list[index];
        }

        /// ***********************************************************************
        /// <summary>
        /// 安定ソート
        /// </summary>
        /// ***********************************************************************
        public static void StableSort<T>( List<T> list, System.Comparison<T> comparison )
        {
            List<KeyValuePair<int,T>> wrapped = new List<KeyValuePair<int,T>>(list.Count);
            for( int i = 0; i < list.Count; i++ )
            {
                wrapped.Add( new KeyValuePair<int,T>( i, list[i] ) );
            }
            
            wrapped.Sort( delegate( KeyValuePair<int,T> x, KeyValuePair<int,T> y )
                {
                    int result = comparison( x.Value, y.Value );
                    if( result == 0 )
                    {
                        result = x.Key.CompareTo( y.Key );
                    }
                    return result;
                });
            
            for( int i = 0; i < list.Count; i++ )
            {
                list[i] = wrapped[i].Value;
            }
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 安定ソート(逆順)
        /// </summary>
        /// ***********************************************************************
        public static void StableSortReverse<T>( List<T> list, System.Comparison<T> comparison )
        {
            List<KeyValuePair<int,T>> wrapped = new List<KeyValuePair<int,T>>(list.Count);
            for( int i = 0; i < list.Count; i++ )
            {
                wrapped.Add( new KeyValuePair<int,T>( i, list[i] ) );
            }
            
            wrapped.Sort( delegate( KeyValuePair<int,T> x, KeyValuePair<int,T> y )
            {
                int result = comparison( y.Value, x.Value );
                if( result == 0 )
                {
                    result = x.Key.CompareTo( y.Key );
                }
                return result;
            });
            
            for( int i = 0; i < list.Count; i++ )
            {
                list[i] = wrapped[i].Value;
            }
        }
        
        /// ***********************************************************************
        /// <summary>
        /// マージ
        /// </summary>
        /// <param name="list">マージ先リスト</param>
        /// <param name="dst">マージ対象</param>
        /// <param name="predicate">比較関数</param>
        /// <param name="enableSame">重複を許可するかどうか</param>
        /// ***********************************************************************
        public static void MergeTo<T>( ref List<T> list, T[] dst, System.Func<T,T,bool> predicate, bool enableSame = true )
        {
            if( dst == null || dst.Length == 0 ) return;
            if( enableSame )
            {
                if( list == null ) list = new List<T>( );
                list.AddRange( dst );
            }
            else
            {
                if( list == null ) list = new List<T>( );
                bool isAdd = false;
                for( int i = 0; i < dst.Length; ++i )
                {
                    isAdd = true;
                    foreach( var item in list )
                    {
                        if( predicate( item, dst[i] ) )
                        {
                            isAdd = false;
                            break;
                        }
                    }
                    if( isAdd ) list.Add( dst[i] );
                }
            }
        }
        public static void MergeTo<T>( ref List<T> list, T dst, System.Func<T,T,bool> predicate, bool enableSame = true )
        {
            if( enableSame )
            {
                if( list == null ) list = new List<T>( );
                list.Add( dst );
            }
            else
            {
                if( list == null ) list = new List<T>( );
                foreach( var item in list )
                {
                    if( predicate( item, dst ) )
                    {
                        return;
                    }
                }
                list.Add( dst );
            }
        }
        
        /// ***********************************************************************
        /// <summary>
        /// マージ
        /// </summary>
        /// <param name="list">マージ先リスト</param>
        /// <param name="dst">マージ対象</param>
        /// <param name="predicate">比較関数</param>
        /// <param name="enableSame">重複を許可するかどうか</param>
        /// ***********************************************************************
        public static void MergeTo<T>( ref List<T> list, List<T> dst, System.Func<T,T,bool> predicate, bool enableSame = true )
        {
            if( dst == null || dst.Count == 0 ) return;
            if( enableSame )
            {
                if( list == null ) list = new List<T>( );
                list.AddRange( dst );
            }
            else
            {
                if( list == null ) list = new List<T>( );
                bool isAdd = false;
                for( int i = 0; i < dst.Count; ++i )
                {
                    isAdd = true;
                    foreach( var item in list )
                    {
                        if( predicate( item, dst[i] ) )
                        {
                            isAdd = false;
                            break;
                        }
                    }
                    if( isAdd ) list.Add( dst[i] );
                }
            }
        }
        
        /// ***********************************************************************
        /// <summary>
        /// マージ
        /// </summary>
        /// <param name="list">マージ先リスト</param>
        /// <param name="dst">マージ対象</param>
        /// <param name="predicate">比較関数</param>
        /// <param name="enableSame">重複を許可するかどうか</param>
        /// ***********************************************************************
        public static List<T> Merge<T>( List<T> list, List<T> dst, System.Func<T,T,bool> predicate, bool enableSame = true )
        {
            if( list == null && dst == null ) return null;
            List<T> result = new List<T>( );
            if( list != null ) result.AddRange( list );
            if( dst != null )
            {
                if( result.Count == 0 )
                {
                    result.AddRange( dst );
                }
                else
                {
                    if( enableSame )
                    {
                        result.AddRange( dst );
                    }
                    else
                    {
                        for( int i = 0; i < dst.Count; ++i )
                        {
                            if( result.FindIndex( ( p1 ) => predicate( p1, dst[i] ) ) == -1 )
                            {
                                result.Add( dst[i] );
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// ***********************************************************************
        /// <summary>
        /// デリゲート戻り値をキーとした重複排除. 自身を返す.
        /// </summary>
        /// ***********************************************************************
        public static List<TItem> UniqueBy<TItem, TKey>(this List<TItem> list, Func<TItem, TKey> keyFunc)
        {
            var pool = InstancePool<HashSet<TKey>>.Shared;
            var set = pool.Rent() ?? new HashSet<TKey>();
            set.Clear();
            for (var i = 0; i < list.Count; ++i)
            {
                var key = keyFunc(list[i]);
                if (set.Contains(key))
                {
                    list.RemoveAt(i);
                    --i;
                }
                else
                {
                    set.Add(key);
                }
            }
            set.Clear();
            pool.Return(set);

            return list;
        }

        /// ***********************************************************************
        /// <summary>
        /// デリゲート戻り値をキーとしたソートを行い、自身を返す(コピーではない).
        /// Linq.OrderBy()を模している. アロケーションは初回以外発生しない.
        /// </summary>
        /// ***********************************************************************
        public static List<TItem> SortBy<TItem, TKey>(this List<TItem> list, Func<TItem, TKey> keyFunc)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));

            var count = list.Count;
            var keys = ArrayPool<TKey>.Shared.Rent(count);
            var items = ArrayPool<TItem>.Shared.Rent(count);
            for (var i = 0; i < count; ++i)
            {
                items[i] = list[i];
                keys[i] = keyFunc(list[i]);
            }
            Array.Sort(keys, items, 0, count);
            for (var i = 0; i < count; ++i)
            {
                list[i] = items[i];
            }
            ArrayPool<TKey>.Shared.Return(keys);
            ArrayPool<TItem>.Shared.Return(items, true);

            return list;
        }

        /// ***********************************************************************
        /// <summary>
        /// デリゲート戻り値をキーとした安定ソートを行い、自身を返す.
        /// </summary>
        /// ***********************************************************************
        public static List<TItem> StableSortBy<TItem, TKey>(this List<TItem> list, Func<TItem, TKey> func)
        {
            var tmp = ListPool<KeyValuePair<int, TItem>>.Rent();
            var count = list.Count;
            for (var i = 0; i < count; ++i)
            {
                tmp.Add(new KeyValuePair<int, TItem>(i, list[i]));
            }

            var pool = InstancePool<StableFuncComaparer<TItem, TKey>>.Shared;
            var comparer = pool.Rent() ?? new StableFuncComaparer<TItem, TKey>();
            comparer.Func = func;
            tmp.Sort(comparer);
            comparer.Func = null;
            pool.Return(comparer);

            for (var i = 0; i < count; ++i)
            {
                list[i] = tmp[i].Value;
            }
            ListPool.Return(ref tmp);

            return list;
        }

        sealed class StableFuncComaparer<TItem, TKey> : IComparer<KeyValuePair<int, TItem>>
        {
            readonly Comparer<TKey> keyComparer = Comparer<TKey>.Default;
            public Func<TItem, TKey> Func;

            public int Compare(KeyValuePair<int, TItem> x, KeyValuePair<int, TItem> y)
            {
                var cmp = keyComparer.Compare(Func(x.Value), Func(y.Value));
                if (cmp != 0)
                {
                    return cmp;
                }
                return x.Key - y.Key;
            }
        }


        public static TItem Find<TItem, TState>(this List<TItem> list, Func<TItem, TState, bool> match, TState state)
        {
            if (list == null)
            {
                return default;
            }

            foreach (var item in list)
            {
                if (match(item, state))
                {
                    return item;
                }
            }

            return default;
        }


        public static TItem FindEquals<TItem, TKey>( this List<TItem> list, Func<TItem, TKey> func, TKey key )
        {
            if (list == null)
            {
                return default(TItem);
            }

            var comparer = EqualityComparer<TKey>.Default;

            foreach (var item in list)
            {
                if (comparer.Equals(func(item), key))
                {
                    return item;
                }
            }

            return default(TItem);
        }

        public static bool AnyEquals<TItem, TKey>( this List<TItem> list, Func<TItem, TKey> func, TKey key )
        {
            if (list == null)
            {
                return false;
            }

            var comparer = EqualityComparer<TKey>.Default;

            foreach (var item in list)
            {
                if (comparer.Equals(func(item), key))
                {
                    return true;
                }
            }

            return false;
        }


        public static int FindIndexEquals<TItem, TKey>( this List<TItem> list, Func<TItem, TKey> func, TKey key )
        {
            if (list == null)
            {
                return -1;
            }

            var comparer = EqualityComparer<TKey>.Default;

            int n = 0;
            foreach (var item in list)
            {
                if (comparer.Equals(func(item), key))
                {
                    return n;
                }

                ++ n;
            }

            return -1;
        }


        public static T FindEquals<T>( this List<T> list, Func<T, int> func, int key )
        {
            if (list == null)
            {
                return default(T);
            }

            foreach (var item in list)
            {
                if (func(item) == key)
                {
                    return item;
                }
            }

            return default(T);
        }


        public static List<T> RemoveWithout<T>( this List<T> list, Func<T, bool> predicate )
        {
            if (list == null)
            {
                return null;
            }

            for (var i = list.Count - 1; i >= 0; --i)
            {
                if (!predicate(list[i]))
                {
                    list.RemoveAt(i);
                }
            }

            return list;
        }


        public static List<T> RemoveWithout<T, TContext>( this List<T> list, Func<T, TContext, bool> predicate, TContext context )
        {
            if (list == null)
            {
                return null;
            }

            for (var i = list.Count - 1; i >= 0; --i)
            {
                if (!predicate(list[i], context))
                {
                    list.RemoveAt(i);
                }
            }

            return list;
        }

  
        public static List<T> AppendRange<T>(this List<T> list, IList<T> otherList)
        {
            var count = otherList.Count;
            if (list.Capacity < list.Count + count)
            {
                // Add()のアロケーションを最低限にするため
                list.Capacity = list.Count + count;
            }

            for (var i = 0; i < count; ++i)
            {
                list.Add(otherList[i]);
            }

            return list;
        }


        public static List<T> AppendRange<T, TIterator>(this List<T> list, TIterator interator)
            where TIterator : IOneTimeIterator<T>
        {
            var count = interator.Count;
            if (list.Capacity < list.Count + count)
            {
                // Add()
                list.Capacity = list.Count + count;
            }

            foreach (var item in interator.Each())
            {
                list.Add(item);
            }

            return list;
        }

        public static T[] RentArray<T>(this List<T> list, out int length)
        {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }

            length = list.Count;
            var array = ArrayPool<T>.Shared.Rent(length);
            list.CopyTo(array);
            return array;
        }


        public static void ClearWithReturn<T>(this List<T> list)
            where T : class
        {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }

            foreach (var item in list)
            {
                if (item != null)
                    InstancePool<T>.Shared.Return(item);
            }

            list.Clear();
        }
    }
}
