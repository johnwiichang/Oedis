using System;
using System.Linq;
using StackExchange.Redis;
using System.Reflection;

namespace Oedis
{
    public partial class RedisSet<TEntity>
    {
        private OedisContext Context;
        private IDatabase Db;

        public RedisSet(ConnectionMultiplexer cmp, OedisContext Context, Int32 index = 0)
        {
            Db = cmp.GetDatabase(index);
            this.Context = Context;
        }

        internal static Type Type { get { return typeof(TEntity); } }

        internal static PropertyInfo Master { get { return typeof(TEntity).GetMasterProperty(); } }

        internal static PropertyInfo Reference { get { return typeof(TEntity).GetReferenceProperty().FirstOrDefault(); } }

        internal static String[] EntityProperties { get { return typeof(TEntity).GetEntityProperties().Select(x => x.Name).ToArray(); } }

        public Int32 Length { get { return Context.Keys(Type.Name).Count(); } }
    }
}
