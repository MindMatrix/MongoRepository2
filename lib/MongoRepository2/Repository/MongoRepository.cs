namespace MongoRepository2
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// Deals with entities in MongoDb.
    /// </summary>
    /// <typeparam name="T">The type contained in the repository.</typeparam>
    /// <typeparam name="TKey">The type used for the entity's Id.</typeparam>
    public class MongoRepository<T, TKey> : IRepository<T, TKey>
        where T : IEntity<TKey>
    {
        protected static readonly TypeInfo _typeInfo = typeof(T).GetTypeInfo();

        private static readonly bool _shouldConvertToObjectId = false;

        static MongoRepository()
        {
            if (typeof(TKey) == typeof(string))
            {
                var fieldInfo = _typeInfo.GetField("Id", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.GetField | BindingFlags.FlattenHierarchy);
                if (fieldInfo != null)
                    _shouldConvertToObjectId = fieldInfo.GetCustomAttribute(typeof(BsonRepresentationAttribute)) != null;
                else
                {
                    var propertyInfo = _typeInfo.GetProperty("Id");
                    if (propertyInfo != null)
                        _shouldConvertToObjectId = propertyInfo.GetCustomAttribute(typeof(BsonRepresentationAttribute)) != null;
                }
            }
        }

        /// <summary>
        /// MongoCollection field.
        /// </summary>
        protected internal IMongoCollection<T> collection;

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// Uses the Default App/Web.Config connectionstrings to fetch the connectionString and Database name.
        /// </summary>
        /// <remarks>Default constructor defaults to "MongoServerSettings" key for connectionstring.</remarks>
        public MongoRepository()
            : this(Util<TKey>.GetDefaultConnectionString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        public MongoRepository(string connectionString)
        {
            this.collection = Util<TKey>.GetCollectionFromConnectionString<T>(connectionString);
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        /// <param name="collectionName">The name of the collection to use.</param>
        public MongoRepository(string connectionString, string collectionName)
        {
            this.collection = Util<TKey>.GetCollectionFromConnectionString<T>(connectionString, collectionName);
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="url">Url to use for connecting to MongoDB.</param>
        public MongoRepository(MongoUrl url)
        {
            this.collection = Util<TKey>.GetCollectionFromUrl<T>(url);
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="url">Url to use for connecting to MongoDB.</param>
        /// <param name="collectionName">The name of the collection to use.</param>
        public MongoRepository(MongoUrl url, string collectionName)
        {
            this.collection = Util<TKey>.GetCollectionFromUrl<T>(url, collectionName);
        }

        ///// <summary>
        ///// Gets the Mongo collection (to perform advanced operations).
        ///// </summary>
        ///// <remarks>
        ///// One can argue that exposing this property (and with that, access to it's Database property for instance
        ///// (which is a "parent")) is not the responsibility of this class. Use of this property is highly discouraged;
        ///// for most purposes you can use the MongoRepositoryManager&lt;T&gt;
        ///// </remarks>
        ///// <value>The Mongo collection (to perform advanced operations).</value>
        //public IMongoCollection<T> Collection
        //{
        //    get { return this.collection; }
        //}

        /// <summary>
        /// Gets the name of the collection
        /// </summary>
        public string CollectionName
        {
            get { return this.collection.CollectionNamespace.CollectionName; }
        }

        /// <summary>
        /// Returns the T by its given id.
        /// </summary>
        /// <param name="id">The Id of the entity to retrieve.</param>
        /// <returns>The Entity T.</returns>
        public virtual T GetById(TKey id)
        {
            return this.collection.FindSync<T>(GetIDFilter(id)).Single();
        }

        /// <summary>
        /// Returns the T by its given id.
        /// </summary>
        /// <param name="id">The Id of the entity to retrieve.</param>
        /// <returns>The Entity T.</returns>
        public async virtual Task<T> GetByIdAsync(TKey id)
        {
            return await (await this.collection.FindAsync<T>(GetIDFilter(id))).SingleAsync();
        }

        ///// <summary>
        ///// Returns the T by its given id.
        ///// </summary>
        ///// <param name="id">The Id of the entity to retrieve.</param>
        ///// <returns>The Entity T.</returns>
        //public virtual T GetById(ObjectId id)
        //{
        //    return this.collection.FindSync<T>(GetIDFilter(id)).Single();
        //}

        /// <summary>
        /// Adds the new entity in the repository.
        /// </summary>
        /// <param name="entity">The entity T.</param>
        /// <returns>The added entity including its new ObjectId.</returns>
        public virtual T Add(T entity)
        {
            this.collection.InsertOne(entity);

            return entity;
        }

        public async virtual Task<T> AddAsync(T entity)
        {
            await this.collection.InsertOneAsync(entity);
            return entity;
        }

        /// <summary>
        /// Adds the new entities in the repository.
        /// </summary>
        /// <param name="entities">The entities of type T.</param>
        //public virtual void Add(IEnumerable<T> entities)
        //{
        //    this.collection.InsertMany(entities);
        //}

        public virtual void Add(IEnumerable<T> entities)
        {
            this.collection.InsertMany(entities);
        }

        /// <summary>
        /// Adds the new entities in the repository.
        /// </summary>
        /// <param name="entities">The entities of type T.</param>
        public async virtual Task AddAsync(IEnumerable<T> entities)
        {
            await this.collection.InsertManyAsync(entities);
        }

        /// <summary>
        /// Upserts an entity
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The upserted entity.</returns>
        public virtual T AddOrUpdate(T entity)
        {
            if (entity.Id == null)
                this.Add(entity);
            else
                this.collection.ReplaceOne(GetIDFilter(entity.Id), entity, new UpdateOptions { IsUpsert = true });
            return entity;
        }

        /// <summary>
        /// Upserts an entity
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The upserted entity.</returns>
        public async virtual Task<T> AddOrUpdateAsync(T entity)
        {
            if (entity.Id == null)
                await this.AddAsync(entity);
            else
                await this.collection.ReplaceOneAsync(GetIDFilter(entity.Id), entity, new UpdateOptions { IsUpsert = true });
            return entity;
        }

        /// <summary>
        /// Upserts a range of entity.
        /// </summary>
        /// <param name="entities">The entities to upserted.</param>
        public virtual void AddOrUpdate(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
                this.collection.ReplaceOne(GetIDFilter(entity.Id), entity, new UpdateOptions { IsUpsert = true });
        }

        /// <summary>
        /// Upserts a range of entity.
        /// </summary>
        /// <param name="entities">The entities to upserted.</param>
        public async virtual Task AddOrUpdateAsync(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
                await this.collection.ReplaceOneAsync(GetIDFilter(entity.Id), entity, new UpdateOptions { IsUpsert = true });
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The updated entity.</returns>
        public virtual T Update(T entity)
        {
            if (entity.Id == null) throw new ArgumentNullException(nameof(entity.Id));
            this.collection.ReplaceOne(GetIDFilter(entity.Id), entity, new UpdateOptions { IsUpsert = false });
            return entity;
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The updated entity.</returns>
        public async virtual Task<T> UpdateAsync(T entity)
        {
            if (entity.Id == null) throw new ArgumentNullException(nameof(entity.Id));
            await this.collection.ReplaceOneAsync(GetIDFilter(entity.Id), entity, new UpdateOptions { IsUpsert = false });
            return entity;
        }


        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        public virtual void Update(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
                this.collection.ReplaceOne(GetIDFilter(entity.Id), entity, new UpdateOptions { IsUpsert = false });
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        public async virtual Task UpdateAsync(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
                await this.collection.ReplaceOneAsync(GetIDFilter(entity.Id), entity, new UpdateOptions { IsUpsert = false });
        }

        /// <summary>
        /// Deletes an entity from the repository by its id.
        /// </summary>
        /// <param name="id">The entity's id.</param>
        public virtual void Delete(TKey id)
        {
            this.collection.DeleteOne(GetIDFilter(id));
        }

        /// <summary>
        /// Deletes an entity from the repository by its id.
        /// </summary>
        /// <param name="id">The entity's id.</param>
        public async virtual Task DeleteAsync(TKey id)
        {
            await this.collection.DeleteOneAsync(GetIDFilter(id));
        }


        ///// <summary>
        ///// Deletes an entity from the repository by its ObjectId.
        ///// </summary>
        ///// <param name="id">The ObjectId of the entity.</param>
        //public virtual void Delete(ObjectId id)
        //{
        //    this.collection.DeleteOne(GetIDFilter(id));
        //}

        ///// <summary>
        ///// Deletes an entity from the repository by its ObjectId.
        ///// </summary>
        ///// <param name="id">The ObjectId of the entity.</param>
        //public async virtual Task DeleteAsync(ObjectId id)
        //{
        //    await this.collection.DeleteOneAsync(GetIDFilter(id));
        //}

        /// <summary>
        /// Deletes the given entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public virtual void Delete(T entity)
        {
            this.Delete(entity.Id);
        }

        /// <summary>
        /// Deletes the given entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public async virtual Task DeleteAsync(T entity)
        {
            await this.DeleteAsync(entity.Id);
        }


        /// <summary>
        /// Deletes the entities matching the predicate.
        /// </summary>
        /// <param name="predicate">The expression.</param>
        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            this.collection.DeleteMany<T>(predicate);
        }

        /// <summary>
        /// Deletes the entities matching the predicate.
        /// </summary>
        /// <param name="predicate">The expression.</param>
        public async virtual Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            await this.collection.DeleteManyAsync<T>(predicate);
        }

        /// <summary>
        /// Deletes all entities in the repository.
        /// </summary>
        public virtual void DeleteAll()
        {
            this.collection.DeleteMany<T>(t => true);
        }

        /// <summary>
        /// Deletes all entities in the repository.
        /// </summary>
        public async virtual Task DeleteAllAsync()
        {
            await this.collection.DeleteManyAsync<T>(t => true);
        }

        /// <summary>
        /// Counts the total entities in the repository.
        /// </summary>
        /// <returns>Count of entities in the collection.</returns>
        public virtual long Count()
        {
            return this.collection.Count(t => true);
        }

        /// <summary>
        /// Counts the total entities in the repository.
        /// </summary>
        /// <returns>Count of entities in the collection.</returns>
        public async virtual Task<long> CountAsync()
        {
            return await this.collection.CountAsync(t => true);
        }

        /// <summary>
        /// Checks if the entity exists for given predicate.
        /// </summary>
        /// <param name="predicate">The expression.</param>
        /// <returns>True when an entity matching the predicate exists, false otherwise.</returns>
        public virtual bool Exists(Expression<Func<T, bool>> predicate)
        {
            return this.collection.AsQueryable<T>().Any(predicate);
        }

        /// <summary>
        /// Checks if the entity exists for given predicate.
        /// </summary>
        /// <param name="predicate">The expression.</param>
        /// <returns>True when an entity matching the predicate exists, false otherwise.</returns>
        public async virtual Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await this.collection.AsQueryable<T>().AnyAsync(predicate);
        }

        private static FilterDefinition<T> GetIDFilter(ObjectId id)
        {
            return Builders<T>.Filter.Eq("_id", id);
        }

        private static FilterDefinition<T> GetIDFilter(TKey id)
        {
            if (_shouldConvertToObjectId)
                return GetIDFilter(new ObjectId(id as string));
            return Builders<T>.Filter.Eq("_id", id);
        }

        #region IQueryable<T>
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator&lt;T&gt; object that can be used to iterate through the collection.</returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return this.collection.AsQueryable<T>().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.collection.AsQueryable<T>().GetEnumerator();
        }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of IQueryable is executed.
        /// </summary>
        public virtual Type ElementType
        {
            get { return this.collection.AsQueryable<T>().ElementType; }
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of IQueryable.
        /// </summary>
        public virtual Expression Expression
        {
            get { return this.collection.AsQueryable<T>().Expression; }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        public virtual IQueryProvider Provider
        {
            get { return this.collection.AsQueryable<T>().Provider; }
        }
        #endregion
    }

    /// <summary>
    /// Deals with entities in MongoDb.
    /// </summary>
    /// <typeparam name="T">The type contained in the repository.</typeparam>
    /// <remarks>Entities are assumed to use strings for Id's.</remarks>
    public class MongoRepository<T> : MongoRepository<T, string>, IRepository<T>
        where T : IEntity<string>
    {
        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// Uses the Default App/Web.Config connectionstrings to fetch the connectionString and Database name.
        /// </summary>
        /// <remarks>Default constructor defaults to "MongoServerSettings" key for connectionstring.</remarks>
        public MongoRepository()
            : base() { }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="url">Url to use for connecting to MongoDB.</param>
        public MongoRepository(MongoUrl url)
            : base(url) { }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="url">Url to use for connecting to MongoDB.</param>
        /// <param name="collectionName">The name of the collection to use.</param>
        public MongoRepository(MongoUrl url, string collectionName)
            : base(url, collectionName) { }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        public MongoRepository(string connectionString)
            : base(connectionString) { }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        /// <param name="collectionName">The name of the collection to use.</param>
        public MongoRepository(string connectionString, string collectionName)
            : base(connectionString, collectionName) { }
    }
}