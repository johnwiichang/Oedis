using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Oedis
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// Get custom attributes contains provided types.
        /// </summary>
        /// <param name="t">Current type</param>
        /// <param name="ts">Custom attributes</param>
        /// <returns>Properties</returns>
        public static IEnumerable<PropertyInfo> GetTheseProperties(this Type t, params Type[] ts)
        {
            return t.GetProperties().Where(p => p.CustomAttributes.Select(x => x.AttributeType).Intersect(ts).Count() > 0);
        }

        /// <summary>
        /// Get custom attributes aside from provided types.
        /// </summary>
        /// <param name="t">Current type</param>
        /// <param name="ts">Custom attributes</param>
        /// <returns>Properties</returns>
        public static IEnumerable<PropertyInfo> GetRestProperties(this Type t, params Type[] ts)
        {
            return t.GetProperties().Where(p => p.CustomAttributes.Select(x => x.AttributeType).Intersect(ts).Count() == 0);
        }

        /// <summary>
        /// Get acurate attribute's all properties from a type.
        /// </summary>
        /// <typeparam name="AT">Limited attribute</typeparam>
        /// <param name="t">Current type</param>
        /// <returns>Properties</returns>
        public static IEnumerable<PropertyInfo> GetCustomProperties<AT>(this Type t) where AT : System.Attribute
        {
            return t.GetProperties()
                .Where(x => x.CustomAttributes.Where(p => p.AttributeType == typeof(AT)).Count() != 0);
        }

        /// <summary>
        /// Get Reference properties.
        /// </summary>
        /// <param name="t">Current type</param>
        /// <returns>Properties</returns>
        public static IEnumerable<PropertyInfo> GetReferenceProperty(this Type t)
        {
            return t.GetCustomProperties<ReferenceAttribute>();
        }

        /// <summary>
        /// Get Master property.
        /// </summary>
        /// <param name="t">Current type</param>
        /// <returns>Property</returns>
        public static PropertyInfo GetMasterProperty(this Type t)
        {
            return t.GetCustomProperties<MasterAttribute>().FirstOrDefault();
        }

        /// <summary>
        /// Get all properties which provided in POCO.
        /// </summary>
        /// <param name="t">Current type</param>
        /// <returns>Properties</returns>
        public static IEnumerable<PropertyInfo> GetEntityProperties(this Type t)
        {
            return t.GetRestProperties(typeof(ExceptAttribute));
        }
    }
}