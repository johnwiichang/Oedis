using StackExchange.Redis;
using System;

namespace Oedis
{
    internal static class BasicExtension
    {
        internal static RedisValue ToRedisValue(this Object o, Type type)
        {
            switch (type.FullName)
            {
                case "StackExchange.Redis.RedisValue":
                    return (RedisValue)o;
                case "System.String":
                    return (String)o;
                case "System.Int32":
                    return (Int32)o;
                case "System.Int16":
                    return (Int16)o;
                case "System.Int64":
                    return (Int64)o;
                case "System.UInt32":
                    return (UInt32)o;
                case "System.UInt16":
                    return (UInt16)o;
                case "System.UInt64":
                    return (UInt64)o;
                case "System.DateTime":
                    return ((DateTime)o).ToString(); ;
                case "System.Guid":
                    return ((Guid)o).ToString();
                case "System.Boolean":
                    return (Boolean)o;
                case "System.Double":
                    return (Double)o;
                case "System.Single":
                    return (Single)o;
                case "System.Decimal":
                    return ((Decimal)o).ToString();
                case "System.Byte":
                    return (Byte)o;
                case "System.Char":
                    return (Char)o;
                default:
                    throw new Exception("Unknow Type.");
            }
        }

        public static Object ToEntityValue(this RedisValue value, Type type)
        {
            switch (type.FullName)
            {
                case "System.String":
                    return (String)value;
                case "System.Int32":
                    return (Int32)value;
                case "System.Int16":
                    return (Int16)value;
                case "System.Int64":
                    return (Int64)value;
                case "System.UInt32":
                    return (UInt32)value;
                case "System.UInt16":
                    return (UInt16)value;
                case "System.UInt64":
                    return (UInt64)value;
                case "System.DateTime":
                    return Convert.ToDateTime(value);
                case "System.Guid":
                    return Guid.Parse(value);
                case "System.Boolean":
                    return (Boolean)value;
                case "System.Double":
                    return (Double)value;
                case "System.Single":
                    return (Single)value;
                case "System.Decimal":
                    return Decimal.Parse(value);
                case "System.Byte":
                    return (Byte)value;
                case "System.Char":
                    return (Char)value;
                default:
                    throw new Exception("Unknow Type.");
            }
        }
    }
}
