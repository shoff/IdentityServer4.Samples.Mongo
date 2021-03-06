﻿namespace MongoDbIdentityServer.Store
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using ChaosMonkey.Guards;
    using Configuration;
    using IdentityModel;
    using Interface;
    using Models;
    using MongoDB.Driver;

    public class CustomUserStore : IMongoDbUserStore
    {
        private readonly IRepository repository;

        public CustomUserStore(IRepository repository)
        {
            this.repository = Guard.IsNotNull(repository, nameof(repository));
        }

        public bool ValidateCredentials(string username, string password)
        {
            var user = this.FindByUsername(username);
            if (user != null)
            {
                return user.Password.Equals(password);
            }

            return false;
        }

        public User FindByUsername(string username)
        {
            //var users = this.repository.Database.GetCollection<User>(Constants.USERS_COLLECTION);
            //var user = users.Find(u => u.Username == username).FirstOrDefault();
            var user = this.repository.Where<User>(u => u.Username == username).FirstOrDefault();
            return user;
        }

        public User FindByExternalProvider(string provider, string userId)
        {
            var users = this.repository.Database.GetCollection<User>(Constants.USERS_COLLECTION);
            var user = users.Find(x =>
                x.ProviderName == provider &&
                x.ProviderSubjectId == userId).FirstOrDefault();
            return user;
        }

        public void Update(User user)
        {
            var collection = this.repository.Database.GetCollection<User>(Constants.USERS_COLLECTION);
            collection.ReplaceOne<User>(u => u.Username == user.Username, user);
        }

        public User AutoProvisionUser(string provider, string userId, List<Claim> claims)
        {
            // create a list of claims that we want to transfer into our store
            var filtered = new List<Claim>();

            foreach (var claim in claims)
            {
                // if the external system sends a display name - translate that to the standard OIDC name claim
                if (claim.Type == ClaimTypes.Name)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, claim.Value));
                }
                // if the JWT handler has an outbound mapping to an OIDC claim use that
                else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type))
                {
                    filtered.Add(new Claim(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type], claim.Value));
                }
                // copy the claim as-is
                else
                {
                    filtered.Add(claim);
                }
            }

            // if no display name was provided, try to construct by first and/or last name
            // ReSharper disable once SimplifyLinqExpression
            if (!filtered.Any(x => x.Type == JwtClaimTypes.Name))
            {
                var first = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value;
                var last = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value;
                if (first != null && last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
                }
                else if (first != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first));
                }
                else if (last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, last));
                }
            }

            // create a new unique subject id
            var sub = CryptoRandom.CreateUniqueId();

            // check if a display name is available, otherwise fallback to subject id
            var name = filtered.FirstOrDefault(c => c.Type == JwtClaimTypes.Name)?.Value ?? sub;

            // create new user
            var user = new User
            {
                SubjectId = sub,
                Username = name,
                ProviderName = provider,
                ProviderSubjectId = userId,
                Claims = filtered.Select(c=> (UserClaim)c).ToList()
            };

            // add user to in-memory store
            this.repository.Add<User>(user);

            return user;
        }

        public User FindBySubjectId(string subjectId)
        {
            Guard.IsNotNullOrWhitespace(subjectId, nameof(subjectId));
            var user = this.repository.Where<User>(u => u.SubjectId == subjectId).FirstOrDefault();
            return user;
        }
    }
}