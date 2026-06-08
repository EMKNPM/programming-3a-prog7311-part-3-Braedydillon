using APIConnectorCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIConnectorCore.Repositories
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(int id);
        Task AddAsync(Client client);
        Task UpdateAsync(Client client);
        Task DeleteAsync(int id);
        bool ClientExists(int id);


        Task<IEnumerable<Client>> SearchClientsAsync(string searchString);
    }
}