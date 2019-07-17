namespace MongoDbIdentityServer.Services
{
    using System.Threading.Tasks;
    using ChaosMonkey.Guards;
    using IdentityServer4.Models;
    using IdentityServer4.Services;
    using Store;

    public class CustomProfileService : IProfileService
    {
        private readonly IMongoDbUserStore userStore;

        public CustomProfileService(
            IMongoDbUserStore userStore)
        {
            this.userStore = Guard.IsNotNull(userStore, nameof(userStore));
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            throw new System.NotImplementedException();
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}