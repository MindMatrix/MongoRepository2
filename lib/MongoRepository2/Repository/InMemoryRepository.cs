using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoRepository2
{
    public class InMemoryRepository<T, TKey> : IRepository<T, TKey>
        where T : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {

        protected static readonly TypeInfo _typeInfo = typeof(T).GetTypeInfo();

        protected internal string collectionName;

        /// <summary>
        /// MongoCollection field.
        /// </summary>
        protected internal List<T> collection;

        /// <summary>
        /// Initializes a new instance of the InMemoryRepository class.
        /// </summary>
        /// <remarks></remarks>
        public InMemoryRepository()
        {
            collection = new List<T>();
            collectionName = Util<TKey>.GetCollectionName<T>();
        }

        /// <summary>
        /// Initializes a new instance of the InMemoryRepository class.
        /// </summary>
        /// <param name="collectionName">The name of the collection to use.</param>
        public InMemoryRepository(string collectionName)
        {
            collection = new List<T>();
            this.collectionName = collectionName;
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
        //public IReadOnlyCollection<T> Collection
        //{
        //    get { return this.collection; }
        //}

        /// <summary>
        /// Gets the name of the collection
        /// </summary>
        public string CollectionName => collectionName;

        protected virtual TKey GenerateID() { return default(TKey); }

        /// <summary>
        /// Returns the T by its given id.
        /// </summary>
        /// <param name="id">The Id of the entity to retrieve.</param>
        /// <returns>The Entity T.</returns>
        public virtual T GetById(TKey id)
        {
            return collection.Find(x => x.Id.Equals(id));
        }

        /// <summary>
        /// Returns the T by its given id.
        /// </summary>
        /// <param name="id">The Id of the entity to retrieve.</param>
        /// <returns>The Entity T.</returns>
        public async virtual Task<T> GetByIdAsync(TKey id)
        {
            return await Task.Run(() => { return collection.Find(x => x.Id.Equals(id)); });
        }

        ///// <summary>
        ///// Returns the T by its given id.
        ///// </summary>
        ///// <param name="id">The Id of the entity to retrieve.</param>
        ///// <returns>The Entity T.</returns>
        //public virtual T GetById(ObjectId id)
        //{
        //    return collection.Find(x => x.Id.Equals(id));
        //}

        /// <summary>
        /// Adds the new entity in the repository.
        /// </summary>
        /// <param name="entity">The entity T.</param>
        /// <returns>The added entity including its new ObjectId.</returns>
        public virtual T Add(T entity)
        {
            if (entity.Id == null) entity.Id = GenerateID();
            collection.Add(entity);
            return entity;
        }

        /// <summary>
        /// Adds the new entity in the repository.
        /// </summary>
        /// <param name="entity">The entity T.</param>
        /// <returns>The added entity including its new ObjectId.</returns>

        public async Task<T> AddAsync(T entity)
        {
            if (entity.Id == null) entity.Id = GenerateID();
            return await Task.Run(() => { collection.Add(entity); return entity; });
        }

        /// <summary>
        /// Upserts the entity in the repository.
        /// </summary>
        /// <param name="entity">The entity T.</param>
        /// <returns>The upserted entity.</returns>
        public virtual T AddOrUpdate(T entity)
        {
            if (entity.Id == null)
                this.Add(entity);
            else
            {
                var find = GetById(entity.Id);
                if (find != null) collection.Remove(find);
                collection.Add(entity);
            }
            return entity;
        }

        /// <summary>
        /// Upserts the entity in the repository.
        /// </summary>
        /// <param name="entity">The entity T.</param>
        /// <returns>The upserted entity.</returns>
        public async Task<T> AddOrUpdateAsync(T entity)
        {
            return await Task.Run(() =>
            {
                if (entity.Id == null)
                    this.Add(entity);
                else
                {
                    var find = GetById(entity.Id);
                    if (find != null) collection.Remove(find);
                    collection.Add(entity);
                }
                return entity;
            });
        }

        /// <summary>
        /// Adds the new entities in the repository.
        /// </summary>
        /// <param name="entities">The entities of type T.</param>
        public virtual void Add(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
                if (entity.Id == null) entity.Id = GenerateID();

            collection.AddRange(entities);
        }

        /// <summary>
        /// Adds the new entities in the repository.
        /// </summary>
        /// <param name="entities">The entities of type T.</param>
        public async virtual Task AddAsync(IEnumerable<T> entities)
        {
            await Task.Run(() =>
            {
                foreach (var entity in entities)
                    if (entity.Id == null) entity.Id = GenerateID();

                collection.AddRange(entities);
            });
        }

        /// <summary>
        /// Upserts the entities in the repository.
        /// </summary>
        /// <param name="entities">The entities of type T.</param>
        public virtual void AddOrUpdate(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
                if (entity.Id == null) entity.Id = GenerateID();

            collection.AddRange(entities);
        }

        /// <summary>
        /// Upserts the entities in the repository.
        /// </summary>
        /// <param name="entities">The entities of type T.</param>
        public async virtual Task AddOrUpdateAsync(IEnumerable<T> entities)
        {
            await Task.Run(() =>
            {
                foreach (var entity in entities)
                    if (entity.Id == null) entity.Id = GenerateID();

                collection.AddRange(entities);
            });
        }

        /// <summary>
        /// Upserts an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The updated entity.</returns>
        public virtual T Update(T entity)
        {
            if (entity.Id == null) throw new ArgumentNullException(nameof(entity.Id));

            var find = GetById(entity.Id);
            if (find != null)
            {
                collection.Remove(find);
                collection.Add(entity);
            }
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
            return await Task.Run(() =>
            {
                var find = GetById(entity.Id);
                if (find != null)
                {
                    collection.Remove(find);
                    collection.Add(entity);
                }
                return entity;
            });
        }

        /// <summary>
        /// Upserts the entities.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        public virtual void Update(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
                Update(entity);
        }

        /// <summary>
        /// Upserts the entities.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        public async virtual Task UpdateAsync(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
                await UpdateAsync(entity);
        }


        /// <summary>
        /// Deletes an entity from the repository by its id.
        /// </summary>
        /// <param name="id">The entity's id.</param>
        public virtual void Delete(TKey id)
        {
            var find = GetById(id);
            if (find != null) collection.Remove(find);
        }

        /// <summary>
        /// Deletes an entity from the repository by its id.
        /// </summary>
        /// <param name="id">The entity's id.</param>
        public async virtual Task DeleteAsync(TKey id)
        {
            await Task.Run(() =>
            {
                var find = GetById(id);
                if (find != null) collection.Remove(find);
            });
        }

        ///// <summary>
        ///// Deletes an entity from the repository by its ObjectId.
        ///// </summary>
        ///// <param name="id">The ObjectId of the entity.</param>
        //public virtual void Delete(ObjectId id)
        //{
        //    var find = GetById(id.ToString());
        //    if (find != null) collection.Remove(find);
        //}

        ///// <summary>
        ///// Deletes an entity from the repository by its ObjectId.
        ///// </summary>
        ///// <param name="id">The ObjectId of the entity.</param>
        //public async virtual Task DeleteAsync(ObjectId id)
        //{
        //    await Task.Run(() =>
        //    {
        //        var find = GetById(id);
        //        if (find != null) collection.Remove(find);
        //    });
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
            await Task.Run(() => { this.Delete(entity.Id); });
        }

        /// <summary>
        /// Deletes the entities matching the predicate.
        /// </summary>
        /// <param name="predicate">The expression.</param>
        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            var compiledPredicate = predicate.Compile();
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                if (compiledPredicate(collection[i]))
                {
                    collection.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Deletes the entities matching the predicate.
        /// </summary>
        /// <param name="predicate">The expression.</param>
        public async virtual Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            await Task.Run(() =>
            {
                var compiledPredicate = predicate.Compile();
                for (int i = collection.Count - 1; i >= 0; i--)
                {
                    if (compiledPredicate(collection[i]))
                    {
                        collection.RemoveAt(i);
                    }
                }
            });
        }

        /// <summary>
        /// Deletes all entities in the repository.
        /// </summary>
        public virtual void DeleteAll()
        {
            collection.Clear();
        }

        /// <summary>
        /// Deletes all entities in the repository.
        /// </summary>
        public async virtual Task DeleteAllAsync()
        {
            await Task.Run(() => { collection.Clear(); });
        }

        /// <summary>
        /// Counts the total entities in the repository.
        /// </summary>
        /// <returns>Count of entities in the collection.</returns>
        public virtual long Count()
        {
            return this.collection.Count;
        }

        /// <summary>
        /// Counts the total entities in the repository.
        /// </summary>
        /// <returns>Count of entities in the collection.</returns>
        public async virtual Task<long> CountAsync()
        {
            return await Task.Run(() => { return this.collection.Count; }); ;
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
            return await Task.Run(() => { return collection.AsQueryable<T>().Any(); });
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
    public class InMemoryRepository<T> : InMemoryRepository<T, string>, IRepository<T>
        where T : IEntity<string>
    {
        protected override string GenerateID() => ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// Uses the Default App/Web.Config connectionstrings to fetch the connectionString and Database name.
        /// </summary>
        /// <remarks>Default constructor defaults to "MongoServerSettings" key for connectionstring.</remarks>
        public InMemoryRepository()
            : base() { }

        /// <summary>
        /// Initializes a new instance of the InMemoryRepository class.
        /// </summary>
        /// <param name="collectionName">The name of the collection to use.</param>
        public InMemoryRepository(string collectionName)
            : base(collectionName) { }

    }
}
