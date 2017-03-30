using System;
using MongoDB.Driver;

namespace MongoRepository.Desktop
{
    public class RepositoryManagerTest : MongoRepositoryTests.RepositoryManagerTest
    {
        public const string _mongourl = "mongodb://localhost/MongoRepositoryManagerDesktopTests";
        protected override string MongoUrl => _mongourl;

        protected override void DropDB()
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
            return new MongoRepository<T, K>();
        }
    }
}
