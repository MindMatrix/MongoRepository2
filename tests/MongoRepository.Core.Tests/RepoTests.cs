using MongoDB.Driver;
using MongoRepository;
using System;

namespace MongoRepository.Core
{
    public  class RepoTests : MongoRepositoryTests.RepositoryTest
    {
        public const string _mongourl = "mongodb://localhost/MongoRepositoryCoreTests";

        public RepoTests()
        {
            this.DropDB();
        }

        public override void Dispose()
        {
            this.DropDB();
        }

        private void DropDB()
        {
            var url = new MongoUrl(_mongourl);
            var client = new MongoClient(url);
            client.DropDatabase(url.DatabaseName);
        }

        protected override IRepository<T> CreateRepository<T>()
        {
            var url = new MongoUrl(_mongourl);
            return new MongoRepository<T>(url);
        }

        protected override IRepository<T> CreateRepository<T>(string collectionName)
        {
            var url = new MongoUrl(_mongourl);
            return new MongoRepository<T>(url, collectionName);
        }

        protected override IRepository<T, K> CreateRepository<T, K>()
        {
            var url = new MongoUrl(_mongourl);
            return new MongoRepository<T, K>(url);
        }
    }
}
