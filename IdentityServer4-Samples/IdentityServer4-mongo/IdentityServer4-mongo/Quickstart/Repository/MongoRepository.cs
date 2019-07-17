namespace QuickstartIdentityServer.Quickstart.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Interface;
    using Microsoft.Extensions.Options;
    using MongoDB.Bson;
    using MongoDB.Driver;

    /// <summary>
    /// Provides functionality  to persist "IdentityServer4.Models" into a given MongoDB
    /// </summary>
    public class MongoRepository : IRepository
    {
        protected static IMongoClient mongoClient;
        protected static IMongoDatabase mongoDatabase;

        public MongoRepository(IOptions<ConfigurationOptions> optionsAccessor)
        {
            var configurationOptions = optionsAccessor.Value;

            mongoClient = new MongoClient(configurationOptions.MongoConnection);
            mongoDatabase = mongoClient.GetDatabase(configurationOptions.MongoDatabaseName);
        }


        public IQueryable<T> All<T>() where T : class, new()
        {
            return mongoDatabase.GetCollection<T>(typeof(T).Name).AsQueryable();
        }

        public IQueryable<T> Where<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return this.All<T>().Where(expression);
        }

        public void Delete<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            _ = mongoDatabase.GetCollection<T>(typeof(T).Name).DeleteMany(predicate);
        }

        public T Single<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return this.All<T>().Where(expression).SingleOrDefault();
        }

        public bool CollectionExists<T>() where T : class, new()
        {
            var collection = mongoDatabase.GetCollection<T>(typeof(T).Name);
            var filter = new BsonDocument();
            var totalCount = collection.CountDocuments(filter);
            return totalCount > 0;
        }

        public void Add<T>(T item) where T : class, new()
        {
            mongoDatabase.GetCollection<T>(typeof(T).Name).InsertOne(item);
        }

        public void Add<T>(IEnumerable<T> items) where T : class, new()
        {
            mongoDatabase.GetCollection<T>(typeof(T).Name).InsertMany(items);
        }
    }
}