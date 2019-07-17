namespace MongoDbIdentityServer.Store
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using IdentityServer4.Models;
    using IdentityServer4.Stores;
    using Interface;

    /// <summary>
    /// Handle consent decisions, authorization codes, refresh and reference tokens
    /// </summary>
    public class CustomPersistedGrantStore : IPersistedGrantStore
    {
        protected IRepository repository;

        public CustomPersistedGrantStore(IRepository repository)
        {
            this.repository = repository;
        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var result = this.repository.Where<PersistedGrant>(i => i.SubjectId == subjectId);
            return Task.FromResult(result.AsEnumerable());
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            var result = this.repository.Single<PersistedGrant>(i => i.Key == key);
            return Task.FromResult(result);
        }

        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            this.repository.Delete<PersistedGrant>(i => i.SubjectId == subjectId && i.ClientId == clientId);
            return Task.CompletedTask;
        }

        public Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            this.repository.Delete<PersistedGrant>(i => i.SubjectId == subjectId && i.ClientId == clientId && i.Type == type);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            this.repository.Delete<PersistedGrant>(i => i.Key == key);
            return Task.CompletedTask;
        }

        public Task StoreAsync(PersistedGrant grant)
        {
            this.repository.Add<PersistedGrant>(grant);
            return Task.CompletedTask;
        }
    }
}
