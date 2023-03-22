using backend_dotnet.Models;
using backend_dotnet.Models.Dtos;

namespace backend_dotnet.Interfaces
{
    public interface IUserRepository
    {
        Task<ViewUser> GetUserById(int id);
        Task<int> InsertUser(User user);
        Task UpdateUser(UpdateUser updateUser);
        Task DeleteUser(RemoveUser removeUser);
        Task<Object?> GetPages(Pager pager);
    }
}