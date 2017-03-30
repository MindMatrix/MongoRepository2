using MongoRepository;
using System;

namespace InMemoryRepository
{
    public  class RepoTests :  MongoRepositoryTests.RepositoryTest
    {
        public RepoTests()
        {
        }

        public override void Dispose()
        {
        }

        protected override IRepository<T> CreateRepository<T>()
        {
            return new InMemoryRepository<T>();
        }

        protected override IRepository<T> CreateRepository<T>(string collectionName)
        {
            return new InMemoryRepository<T>(collectionName);
        }

        protected override IRepository<T, K> CreateRepository<T, K>()
        {
            return new InMemoryRepository<T, K>();

        }
    }
}
