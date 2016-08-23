using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Oedis
{
    public partial class RedisSet<TEntity>
    {
        /// <summary>
        /// Find an object using its Master property.
        /// </summary>
        /// <typeparam name="KT">Property type</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Entity</returns>
        public TEntity Find<KT>(KT key)
        {
            return Db.HashGetAll($"{Type.Name}:{key.ToRedisValue(typeof(KT))}").GenerateObject<TEntity>();
        }

        /// <summary>
        /// Find objects using its Master property.
        /// If you need count the mount, please call the Count method rather than call Linq queries after this segment.
        /// </summary>
        /// <param name="predicate">The logic which used to compare.</param>
        /// <param name="from">Start</param>
        /// <param name="to">Termination</param>
        /// <returns>Entities</returns>
        public IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> predicate, Int32 from = 0, Int32 to = -1)
        {
            NeedReference();
            // Parse the expression tree, find out the reference value.
            var ReferenceVal = predicate.Body.GetValue(typeof(TEntity), Reference.Name);
            // Get all object pointers from reference records list
            var strs = Db.ListRange($"{Type.Name}.{Reference.Name}:{ReferenceVal}", from, to);
            List<TEntity> lst = new List<TEntity>();
            List<Task> tasks = new List<Task>();
            foreach (var item in strs)
            {
                tasks.Add(Task.Run(() => lst.Add(Find(item))));
            }
            Task.WaitAll(tasks.ToArray());
            lst.RemoveAll(x => x == null);
            return lst;
        }

        /// <summary>
        /// Inser an object into Db.
        /// </summary>
        /// <param name="o">Object</param>
        /// <returns></returns>
        public void Add(params TEntity[] o)
        {
            foreach (var item in o)
            {
                if (Reference != null)
                {
                    // Add records into reference list.
                    Task.Run(() => Db.ListRightPush($"{typeof(TEntity).Name}.{Reference.Name}:{Reference.GetValue(item).ToRedisValue(Reference.PropertyType)}", Master.GetValue(item).ToRedisValue(Master.PropertyType)));
                }
                // Add object k-v into redis as hashset.
                Task.Run(() => Db.HashSet($"{typeof(TEntity).Name}:{Master.GetValue(item).ToRedisValue(Master.PropertyType)}", item.ToHashEntries()));
            }
        }

        /// <summary>
        /// Remove an object from Db.
        /// </summary>
        /// <param name="o">Object</param>
        /// <returns></returns>
        public void Remove<KT>(params KT[] o)
        {
            var lst = new List<RedisValue>();
            var tasks = new List<Task>();
            if (Reference != null)
            {
                if (typeof(KT) == Type)
                {
                    foreach (var item in o)
                    {
                        lst.Add(Reference.GetValue(item).ToRedisValue(Reference.PropertyType));
                    };
                }
                else
                {
                    foreach (var item in o)
                    {
                        // Find out all references.
                        lst.Add(Db.HashGet($"{typeof(TEntity).Name}:{item.ToRedisValue(typeof(KT))}", Reference.Name));
                    }
                }
            }
            for (int i = 0; i < o.Length; i++)
            {
                RedisValue key;
                if (typeof(KT) == Type)
                {
                    key = Master.GetValue(o[i]).ToRedisValue(Master.PropertyType);
                }
                else
                {
                    key = o[i].ToRedisValue(typeof(KT));
                }
                if (lst.Count != 0)
                {
                    // Remove list record.
                    tasks.Add(Db.ListRemoveAsync($"{typeof(TEntity).Name}.{Reference.Name}:{lst[i]}", key));
                }
                // Remove hashset.
                tasks.Add(Db.KeyDeleteAsync($"{typeof(TEntity).Name}:{key}"));
            }
            Task.WaitAll();
        }

        /// <summary>
        /// Delete all objects according to Reference property.
        /// </summary>
        /// <typeparam name="predicate">The logic which used to compare.</typeparam>
        /// <returns></returns>
        public void Clear(Expression<Func<TEntity, bool>> predicate)
        {
            // Ensure reference property is existed.
            NeedReference();
            // Parse the expression tree, find out the reference value.
            var ReferenceVal = predicate.Body.GetValue(typeof(TEntity), Reference.Name);
            // Find out all object records.
            var strs = Db.ListRange($"{Type.Name}.{Reference.Name}:{ReferenceVal}");
            var tasklst = new List<Task>();
            foreach (var item in strs)
            {
                // Delete object k-v records.
                tasklst.Add(Db.KeyDeleteAsync($"{typeof(TEntity).Name}:{item}"));
            }
            // Delete reference list
            tasklst.Add(Db.KeyDeleteAsync($"{typeof(TEntity).Name}.{Reference.Name}:{ReferenceVal}"));
            Task.WaitAll(tasklst.ToArray());
        }

        /// <summary>
        /// Count the mount according to Reference property.
        /// </summary>
        /// <param name="predicate">The logic which used to compare.</param>
        /// <returns>Number of reocrds</returns>
        public Int64 Count(Expression<Func<TEntity, bool>> predicate)
        {
            NeedReference();
            var ReferenceVal = predicate.Body.GetValue(typeof(TEntity), Reference.Name);
            return Db.ListLength($"{Type.Name}.{Reference.Name}:{ReferenceVal}");
        }

        /// <summary>
        /// Update object.
        /// </summary>
        /// <param name="o">New object</param>
        /// <returns></returns>
        public void Update(TEntity o)
        {
            // Delete old hashset.
            Db.KeyDelete($"{typeof(TEntity).Name}:{Master.GetValue(o)}");
            // Set a new one.
            Db.HashSet($"{typeof(TEntity).Name}:{Master.GetValue(o).ToRedisValue(Master.PropertyType)}",
                                o.ToHashEntries());
        }

        /// <summary>
        /// Tell the method need reference or not.
        /// </summary>
        private void NeedReference()
        {
            if (Reference == null)
            {
                throw new Exception("该实体无 Reference 属性！");
            }
        }
    }
}
