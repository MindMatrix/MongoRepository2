namespace MongoRepository2.Tests
{
    using Xunit;
    using MongoDB.Driver;
    using MongoRepository2.Tests.Entities;

    public abstract class AbstractRepositoryManagerTests : AbstractRepository
    {
        public AbstractRepositoryManagerTests()
        {
            this.DropDB();
        }

        public override void Dispose()
        {
            this.DropDB();
        }

        protected abstract void DropDB();
        protected abstract string MongoUrl { get; }

        #region Exists

        [Fact]
        public void NotExists()
        {
            var em = new MongoRepositoryManager<Product>(MongoUrl);
            Assert.False(em.Exists());
        }

        [Fact]
        public async void NotExistsAsync()
        {
            var em = new MongoRepositoryManager<Product>(MongoUrl);
            Assert.False(await em.ExistsAsync());
        }

        [Fact]
        public void Exists()
        {
            var products = CreateRandomRepository<Product>();
            var productManager = new MongoRepositoryManager<Product>(MongoUrl, products.CollectionName);

            products.Add(new Product() { Name = "Product 10", Description = "Product 10", Price = 10 });
            Assert.True(productManager.Exists());
        }

        [Fact]
        public async void ExistsAsync()
        {
            var products = CreateRandomRepository<Product>();
            var productManager = new MongoRepositoryManager<Product>(MongoUrl, products.CollectionName);

            await products.AddAsync(new Product() { Name = "Product 10", Description = "Product 10", Price = 10 });
            Assert.True(await productManager.ExistsAsync());
        }
        #endregion Exists

        #region Drop

        [Fact]
        public void Drop()
        {
            var products = CreateRandomRepository<Product>();
            var productManager = new MongoRepositoryManager<Product>(MongoUrl, products.CollectionName);

            products.Add(new Product() { Name = "Product 10", Description = "Product 10", Price = 10 });
            Assert.True(productManager.Exists());
            productManager.Drop();
            Assert.False(productManager.Exists());
        }

        [Fact]
        public async void DropAsync()
        {
            var products = CreateRandomRepository<Product>();
            var productManager = new MongoRepositoryManager<Product>(MongoUrl, products.CollectionName);

            await products.AddAsync(new Product() { Name = "Product 10", Description = "Product 10", Price = 10 });
            Assert.True(await productManager.ExistsAsync());
            await productManager.DropAsync();
            Assert.False(await productManager.ExistsAsync());
        }

        #endregion Drop

        #region Capped

        [Fact]
        public void AutoCreatedCollectionIsNotCapped()
        {
            var products = CreateRandomRepository<Product>();
            var productManager = new MongoRepositoryManager<Product>(MongoUrl, products.CollectionName);

            products.Add(new Product() { Name = "Product 10", Description = "Product 10", Price = 10 });
            Assert.False(productManager.IsCapped());
        }

        [Fact]
        public void IsCapped()
        {
            var products = CreateRandomRepository<Product>();
            var mongoUrl = new MongoUrl(MongoUrl);
            var client = new MongoClient(mongoUrl);
            var options = new CreateCollectionOptions() { Capped = true, MaxSize = 1024768, MaxDocuments = 10000 };
            var database = client.GetDatabase(mongoUrl.DatabaseName);
            database.CreateCollection(products.CollectionName, options);

            products.Add(new Product() { Name = "Product 10", Description = "Product 10", Price = 10 });

            var productManager = new MongoRepositoryManager<Product>(MongoUrl, products.CollectionName);
            Assert.True(productManager.IsCapped());
        }

        [Fact]
        public async void IsCappedAsync()
        {
            var products = CreateRandomRepository<Product>();
            var mongoUrl = new MongoUrl(MongoUrl);
            var client = new MongoClient(mongoUrl);
            var options = new CreateCollectionOptions() { Capped = true, MaxSize = 1024768, MaxDocuments = 10000 };
            var database = client.GetDatabase(mongoUrl.DatabaseName);
            await database.CreateCollectionAsync(products.CollectionName, options);

            await products.AddAsync(new Product() { Name = "Product 10", Description = "Product 10", Price = 10 });

            var productManager = new MongoRepositoryManager<Product>(MongoUrl, products.CollectionName);
            Assert.True(await productManager.IsCappedAsync());
        }

        #endregion Capped

        #region EnsureIndex

        [Fact]
        public void EnsureIndex()
        {

        }

        #endregion EnsureIndex
    }
}
