using System.Collections.Generic;
using System.Threading;

namespace EG
{
    public class InstancePool<T> : InstancePool.IClearablePool
        where T : class
    {
#if UNITY_EDITOR
        public static bool IsConcurrentSupport = false;
#else
        const bool IsConcurrentSupport = false;
#endif

        public static readonly InstancePool<T> Shared = new InstancePool<T>();
        SpinLock spinLock;
        List<T> pool = null;
        int max = 4;
        int lack = 0;
        int over = 0;
        int balance = 0;

        InstancePool()
        {
            lock (InstancePool.pools)
            {
                InstancePool.pools.Add(this);
            }
        }

        public T Rent()
        {
            var locked = false;
            try
            {
                if (IsConcurrentSupport)
                    spinLock.Enter(ref locked);

                if (pool == null || pool.Count == 0)
                {
                    if (over > 0)
                    {
                        over--;
                        balance++;
                    }
                    else
                    {
                        lack++;
                    }
                    return null;
                }

                var instance = pool[0];
                pool.RemoveAt(0);
                return instance;
            }
            finally
            {
                if (locked) spinLock.Exit();
            }
        }

        public void Return(T instance)
        {
            var locked = false;
            try
            {
                if (IsConcurrentSupport)
                    spinLock.Enter(ref locked);

                pool = pool ?? new List<T>();

                if (pool.Count >= max)
                {
                    if (lack > 0)
                    {
                        lack--;
                        balance++;
                    }
                    else
                    {
                        over++;
                    }

                    if (max < 64 && balance > max * 2)
                    {
                        max *= 2;
                        balance = 0;
                    }
                    else
                    {
                        return;
                    }
                }
                pool.Add(instance);
            }
            finally
            {
                if (locked) spinLock.Exit();
            }
        }

        public void Clear()
        {
            pool = null;
            max = 4;
            lack = over = balance = 0;
        }
    }

    public static class InstancePool
    {
        public interface IClearablePool
        {
            void Clear();
        }

        public static HashSet<IClearablePool> pools = new HashSet<IClearablePool>();

        public static void Clear()
        {
            foreach (var pool in pools)
            {
                pool.Clear();
            }
        }
    }
}
