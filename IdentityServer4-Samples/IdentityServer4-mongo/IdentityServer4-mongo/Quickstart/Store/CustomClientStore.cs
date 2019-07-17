using System.Threading.Tasks;
using IdentityServer4.Models;
using QuickstartIdentityServer.Quickstart.Interface;

namespace QuickstartIdentityServer.Quickstart.Store
{
    public class CustomClientStore : IdentityServer4.Stores.IClientStore
    {
        protected IRepository repository;

        public CustomClientStore(IRepository repository)
        {
            this.repository = repository;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = this.repository.Single<Client>(c => c.ClientId == clientId);

            return Task.FromResult(client);
        }
    }
}
