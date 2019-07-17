namespace MongoDbIdentityServer.Store
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using Models;

    public interface IMongoDbUserStore
    {
        bool ValidateCredentials(string username, string password);
        User FindByUsername(string username);
        User FindByExternalProvider(string provider, string userId);
        User AutoProvisionUser(string provider, string userId, List<Claim> claims);
    }
}