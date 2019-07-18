namespace MongoDbIdentityServer.Services
{
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using ChaosMonkey.Guards;
    using IdentityServer4.Extensions;
    using IdentityServer4.Models;
    using IdentityServer4.Services;
    using Microsoft.Extensions.Logging;
    using Store;

    public class CustomProfileService : IProfileService
    {
        private readonly ILogger logger;
        private readonly IMongoDbUserStore userStore;
                                        
        public CustomProfileService(
            IMongoDbUserStore userStore)
        {
            this.userStore = Guard.IsNotNull(userStore, nameof(userStore));
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            if (context.RequestedClaimTypes.Any())
            {
                var user = this.userStore.FindBySubjectId(context.Subject.GetSubjectId());
                if (user != null)
                {
                    context.AddRequestedClaims(user.Claims.Select(c=>(Claim)c).ToList());
                }
            }
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            var user = this.userStore.FindByUsername(context.Subject.Identity.Name);
            if (user == null)
            {
                return Task.CompletedTask;
            }

            if (user.IsActive != context.IsActive)
            {
                user.IsActive = context.IsActive;
                this.userStore.Update(user);
            }

            return Task.CompletedTask;
        }
    }
}