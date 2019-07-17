namespace MongoDbIdentityServer
{
    using System.Collections.Generic;
    using IdentityServer4;
    using IdentityServer4.Models;
    using Models;

    public class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API")
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {"api1"}
                },

                // resource owner password grant client
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {"api1"}
                },

                // OpenID Connect hybrid flow and client credentials client (MVC)
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris = {"http://localhost:5002/signin-oidc"},
                    PostLogoutRedirectUris = {"http://localhost:5002"},

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    },
                    AllowOfflineAccess = true
                }
            };
        }

        public static List<User> GetUsers()
        {
            return new List<User>
            {
                new User
                {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "password",

                    Claims = new List<UserClaim>
                    {
                        new UserClaim("name", "Alice"),
                        new UserClaim("website", "https://alice.com")
                    }
                },
                new User
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password",

                    Claims = new List<UserClaim>
                    {
                        new UserClaim("name", "Bob"),
                        new UserClaim("website", "https://bob.com")
                    }
                }
            };
        }
    }
}