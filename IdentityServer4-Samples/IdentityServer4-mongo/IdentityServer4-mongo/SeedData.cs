namespace MongoDbIdentityServer
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using ChaosMonkey.Guards;
    using IdentityModel;
    using IdentityServer4;
    using Interface;
    using Models;

    public class SeedData
    {
        private readonly IRepository repository;

        public SeedData(
            IRepository repository)
        {
            this.repository = Guard.IsNotNull(repository, nameof(repository));
        }

        public void Seed()
        {
            foreach (var user in users)
            {
                var existing = this.repository.Where<User>(u => u.SubjectId == user.SubjectId).FirstOrDefault();

                if (existing == null)
                {
                    this.repository.Add<User>(user);
                }
            }
        }

        public static List<User> users = new List<User>
        {
            new User{SubjectId = "818727", Username = "alice", Password = "alice",
                Claims =
                {
                    new Claim(JwtClaimTypes.Name, "Alice Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Alice"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                    new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServerConstants.ClaimValueTypes.Json)
                }
            },
            new User{SubjectId = "88421113", Username = "bob", Password = "bob",
                Claims =
                {
                    new Claim(JwtClaimTypes.Name, "Bob Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Bob"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                    new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServerConstants.ClaimValueTypes.Json),
                    new Claim("location", "somewhere"),
                }
            },
        };
    }
}
