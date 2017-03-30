using MongoDB.Bson;
using MongoRepository;
using System;

namespace MongoRepositoryTests
{
    public abstract class RepositoryBase : IDisposable
    {
        public abstract void Dispose();

        protected abstract IRepository<T> CreateRepository<T>()
            where T : IEntity<string>;

        protected IRepository<T> CreateRandomRepository<T>()
            where T : IEntity<string>
        {
            return CreateRepository<T>(ObjectId.GenerateNewId().ToString());
        }

        protected abstract IRepository<T> CreateRepository<T>(string collectionName)
            where T : IEntity<string>;

        protected abstract IRepository<T, K> CreateRepository<T, K>()
            where T : IEntity<K>
            where K : IEquatable<K>;

    }
}
