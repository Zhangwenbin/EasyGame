/**************************************************************************/
/*! @file   OneTimeIterator.cs
    @brief  辞書ユーティリティ
***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using System;

namespace EG
{

    public interface IOneTimeIterator<T> : IOneTimeIterator
    {
        int Count { get; }
        T Current { get; }
        bool MoveNext();
        void Dispose();
    }

    public interface IOneTimeIterator
    {
    }

    public static class OneTimeIteratorExtension
    {
        public static OneTimeIteratorEnumerable<TIterator> Each<TIterator>(this TIterator iterator)
            where TIterator : IOneTimeIterator
        {
            return new OneTimeIteratorEnumerable<TIterator>(iterator);
        }
        
        public static ListPoolOneTimeIterator<T> OneTime<T>(this List<T> list)
        {
            return new ListPoolOneTimeIterator<T>(list);
        }
    }

    public struct OneTimeIteratorEnumerable<T>
    {
        T iterator;
        public OneTimeIteratorEnumerable(T iterator) { this.iterator = iterator; }
        public T GetEnumerator() { return iterator; }
    }

    public struct ListPoolOneTimeIterator<T> : IDisposable, IOneTimeIterator<T>
    {
        List<T> list;
        List<T>.Enumerator e;
        public int Count => list.Count;

        public ListPoolOneTimeIterator(List<T> list)
        {
            this.list = list;
            e = list.GetEnumerator();
        }

        public ListPoolOneTimeIterator<T> GetEnumerator()
        {
            return this;
        }

        public T Current => e.Current;

        public bool MoveNext()
        {
            return e.MoveNext();
        }

        public void Dispose()
        {
            ListPool<T>.Return(list);
            list = null;
        }
    }
}
