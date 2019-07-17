namespace MongoDbIdentityServer.Store
{
    using System.Threading.Tasks;
    using IdentityServer4.Models;
    using IdentityServer4.Stores;
    using Interface;

    public class CustomUserConsentStore : IUserConsentStore
    {
        private readonly IRepository repository;

        public CustomUserConsentStore(IRepository repository)
        {
            this.repository = repository;
        }

        public Task StoreUserConsentAsync(Consent consent)
        {
            throw new System.NotImplementedException();
        }

        public Task<Consent> GetUserConsentAsync(string subjectId, string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveUserConsentAsync(string subjectId, string clientId)
        {
            throw new System.NotImplementedException();
        }
    }
}