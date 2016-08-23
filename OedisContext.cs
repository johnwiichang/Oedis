using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace Oedis
{
    public class OedisContext
    {
        private ConnectionMultiplexer Connection;

        private IServer Server;

        private Int32 DbIndex;

        public OedisContext(String host = "localhost", Int32 port = 6379, Int32 DbIndex = 0)
        {
            Connection = ConnectionMultiplexer.Connect($"{host}:{port}");
            this.DbIndex = DbIndex;
            Server = Connection.GetServer($"{host}:{port}");
            foreach (var item in this.GetType().GetProperties())
            {
                item.SetValue(this, item.PropertyType.GetConstructor(new Type[] { typeof(ConnectionMultiplexer), typeof(OedisContext), typeof(Int32) }).Invoke(new Object[] { Connection, this, DbIndex }));
            }
        }

        public IEnumerable<RedisKey> Keys(String Pattern)
        {
            return Server.Keys(DbIndex, Pattern);
        }
    }
}
