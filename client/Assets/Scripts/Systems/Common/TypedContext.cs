using System;
using System.Collections.Generic;

namespace EG
{
    public sealed class TypedContext : IDisposable
    {
        readonly Dictionary<Type, object> dictionary;

        TypedContext()
        {
            dictionary = new Dictionary<Type, object>();
        }

        public static TypedContext Create()
        {
            return InstancePool<TypedContext>.Shared.Rent() ?? new TypedContext();
        }

        public void Dispose()
        {
            dictionary.Clear();
            InstancePool<TypedContext>.Shared.Return(this);
        }

        public T Get<T>() where T : class
        {
            if (dictionary.TryGetValue(typeof(T), out var value))
            {
                return (T)value;
            }
            return null;
        }

        public T GetOrCreate<T>() where T : class, new()
        {
            if (dictionary.TryGetValue(typeof(T), out var value))
            {
                return (T)value;
            }
            value = new T();
            dictionary.Add(typeof(T), value);
            return (T)value;
        }

        public void Set<T>(T value) where T : class
        {
            dictionary[typeof(T)] = value;
        }
    }
}
