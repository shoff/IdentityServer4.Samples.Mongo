using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using QuickstartIdentityServer.Quickstart.Interface;

namespace QuickstartIdentityServer.Quickstart.Store
{
    public class CustomResourceStore : IResourceStore
    {
        protected IRepository repository;

        public CustomResourceStore(IRepository repository)
        {
            this.repository = repository;
        }

        private IEnumerable<ApiResource> GetAllApiResources()
        {
            return this.repository.All<ApiResource>();
        }

        private IEnumerable<IdentityResource> GetAllIdentityResources()
        {
            return this.repository.All<IdentityResource>();
        }

        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return Task.FromResult(this.repository.Single<ApiResource>(a => a.Name == name));
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var list = this.repository.Where<ApiResource>(a => a.Scopes.Any(s => scopeNames.Contains(s.Name)));

            return Task.FromResult(list.AsEnumerable());
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var list = this.repository.Where<IdentityResource>(e => scopeNames.Contains(e.Name));

            return Task.FromResult(list.AsEnumerable());
        }

        public Resources GetAllResources()
        {
            var result = new Resources(this.GetAllIdentityResources(), this.GetAllApiResources());
            return result;
        }

        private Func<IdentityResource, bool> BuildPredicate(Func<IdentityResource, bool> predicate)
        {
            return predicate;
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            var result = new Resources(this.GetAllIdentityResources(), this.GetAllApiResources());
            return Task.FromResult(result);
        }
    }
}
