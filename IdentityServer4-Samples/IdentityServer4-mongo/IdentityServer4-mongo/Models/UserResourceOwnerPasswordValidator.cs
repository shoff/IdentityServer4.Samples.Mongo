namespace MongoDbIdentityServer.Models
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using ChaosMonkey.Guards;
    using IdentityModel;
    using IdentityServer4.Validation;
    using Microsoft.AspNetCore.Authentication;
    using Store;

    /// <summary>
    /// Resource owner password validator for test users
    /// </summary>
    /// <seealso cref="IdentityServer4.Validation.IResourceOwnerPasswordValidator" />
    public class UserResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IMongoDbUserStore users;
        private readonly ISystemClock clock;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityServer4.Test.TestUserResourceOwnerPasswordValidator"/> class.
        /// </summary>
        /// <param name="users">The users.</param>
        /// <param name="clock">The clock.</param>
        public UserResourceOwnerPasswordValidator(
            IMongoDbUserStore users, 
            ISystemClock clock)
        {
            this.users = Guard.IsNotNull(users, nameof(users));
            this.clock = Guard.IsNotNull(clock, nameof(clock));
        }

        /// <summary>
        /// Validates the resource owner password credential
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (this.users.ValidateCredentials(context.UserName, context.Password))
            {
                var user = this.users.FindByUsername(context.UserName);

                if (string.IsNullOrWhiteSpace(user.SubjectId))
                {
                    throw new ArgumentException("Subject ID not set", nameof(user.SubjectId));
                }

                context.Result = new GrantValidationResult(
                    user.SubjectId,
                    OidcConstants.AuthenticationMethods.Password,
                    this.clock.UtcNow.UtcDateTime, 
                    user.Claims.Select(c=> (Claim)c));
            }

            return Task.CompletedTask;
        }
    }
}