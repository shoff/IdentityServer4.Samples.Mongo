namespace MongoDbIdentityServer.Configuration
{
    using System;
    using IdentityServer4.Models;
    using Interface;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using MongoDB.Bson.Serialization;

    public static class App
    {
        internal static IApplicationBuilder InitializeDatabase(this IApplicationBuilder app)
        {
            var createdNewRepository = false;
            var repository = app.ApplicationServices.GetService<IRepository>();

            //  --Client
            if (!repository.CollectionExists<Client>())
            {
                foreach (var client in Config.GetClients())
                {
                    repository.Add(client);
                }

                createdNewRepository = true;
            }

            //  --IdentityResource
            if (!repository.CollectionExists<IdentityResource>())
            {
                foreach (var res in Config.GetIdentityResources())
                {
                    repository.Add(res);
                }

                createdNewRepository = true;
            }


            //  --ApiResource
            if (!repository.CollectionExists<ApiResource>())
            {
                foreach (var api in Config.GetApiResources())
                {
                    repository.Add(api);
                }

                createdNewRepository = true;
            }

            // If it's a new Repository (database), need to restart the website to configure Mongo to ignore Extra Elements.
            if (createdNewRepository)
            {
                // WTF is this bullshit?
                var newRepositoryMsg =
                    "Mongo Repository created/populated! Please restart you website, so Mongo driver will be configured  to ignore Extra Elements.";
                throw new Exception(newRepositoryMsg);
            }
            return app;
        }

        /// <summary>
        /// Configure Classes to ignore Extra Elements (e.g. _Id) when deserializing
        /// As we are using "IdentityServer4.Models" we cannot add something like "[BsonIgnore]"
        /// </summary>
        internal static IApplicationBuilder ConfigureMongoDriver2IgnoreExtraElements(this IApplicationBuilder app)
        {
            // --- Configure Classes to ignore Extra Elements (e.g. _Id) when deserializing ---

            BsonClassMap.RegisterClassMap<Client>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<IdentityResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<ApiResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<PersistedGrant>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            return app;
        }
    }
}