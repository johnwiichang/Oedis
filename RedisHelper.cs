using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Oedis
{
    public static class RedisHelper
    {
        /// <summary>
        /// Convert object to Redis Key-Value pairs.
        /// </summary>
        /// <typeparam name="T">Origin type</typeparam>
        /// <param name="entity">Source object</param>
        /// <returns></returns>
        public static HashEntry[] ToHashEntries<T>(this T entity)
        {
            var hash = new List<HashEntry>();
            var properties = typeof(T).GetEntityProperties();
            foreach (var item in properties)
            {
                hash.Add(new HashEntry(item.Name, item.GetValue(entity).ToRedisValue(item.PropertyType)));
            }
            return hash.ToArray();
        }

        /// <summary>
        /// Serialize the Redis Key-Value pairs to object.
        /// </summary>
        /// <typeparam name="T">Expected type</typeparam>
        /// <param name="hashs">Current Redis Key-Value pairs</param>
        /// <returns>Entity</returns>
        public static T GenerateObject<T>(this HashEntry[] hashs)
        {
            var properties = typeof(T).GetEntityProperties();
            Assembly assembly = Assembly.GetAssembly(typeof(T));
            Object o = assembly.CreateInstance(typeof(T).FullName);
            foreach (var item in properties)
            {
                item.SetValue(o, hashs.First(x => x.Name == item.Name).Value.ToEntityValue(item.PropertyType));
            }
            return (T)o;
        }
    }
}
