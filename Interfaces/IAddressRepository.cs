using backend_dotnet.Models;

namespace backend_dotnet.Interfaces
{
    public interface IAddressRepository
    {
        Task<Address> GetAddressById(int id);
        Task<int> InsertAddress(Address address);
        Task UpdateAddress(Address address, int id);
    }
}