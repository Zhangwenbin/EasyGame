/**************************************************************************/
/*! @file   ReflectionUtility.cs
    @brief  ReflectionUtility
***************************************************************************/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace EG
{
    //=========================================================================
    //. ReflectionUtility
    //=========================================================================
    public static class ReflectionUtility
    {
        static class AttributeCache<T>
            where T : Attribute
        {
            static Dictionary<Type, T[]> typeCache;
            internal static Dictionary<Type, T[]> TypeCache => typeCache ?? (typeCache = new Dictionary<Type, T[]>());

            static Dictionary<FieldInfo, T[]> fieldCache;
            internal static Dictionary<FieldInfo, T[]> FieldCache => fieldCache ?? (fieldCache = new Dictionary<FieldInfo, T[]>());
        }

        static class GenericCache<T>
        {
            internal static FieldInfo[] FieldsCache = null;
        }

        static Dictionary<Type, FieldInfo[]> fieldsCache;
        static Dictionary<Type, FieldInfo[]> FieldsCache => fieldsCache ?? (fieldsCache = new Dictionary<Type, FieldInfo[]>());

        public static T[] GetCustomAttributes<T>(Type type)
            where T : Attribute
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var cache = AttributeCache<T>.TypeCache;
            if (cache.TryGetValue(type, out var attrs))
            {
                return attrs;
            }

            attrs = (T[])type.GetCustomAttributes(typeof(T), true);
            cache[type] = attrs;
            return attrs;
        }

        public static T[] GetCustomAttributes<T>(FieldInfo field)
            where T : Attribute
        {
            if (field == null) throw new ArgumentNullException(nameof(field));

            var cache = AttributeCache<T>.FieldCache;
            if (cache.TryGetValue(field, out var attrs))
            {
                return attrs;
            }

            attrs = (T[])field.GetCustomAttributes(typeof(T), true);
            cache[field] = attrs;
            return attrs;
        }
        
        public static T GetCustomAttribute<T>(FieldInfo field)
            where T : Attribute
        {
            var attr = GetCustomAttributes<T>(field);
            return attr.Length > 0 ? attr[0] : null;
        }

        public static FieldInfo[] GetFields(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var cache = FieldsCache;
            if (cache.TryGetValue(type, out var fields))
            {
                return fields;
            }

            fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            cache[type] = fields;
            return fields;
        }

        public static FieldInfo[] GetFields<T>()
        {
            var fields = GenericCache<T>.FieldsCache;
            if (fields != null)
            {
                return fields;
            }

            var type = typeof(T);
            fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            FieldsCache[type] = fields;
            return fields;
        }

        public static List<FieldInfo> GetPublicFields(Type type)
        {
            return GetFields(type).Filter(x => x.IsPublic);
        }
    }
}
