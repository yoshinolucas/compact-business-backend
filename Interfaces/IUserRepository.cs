using backend_dotnet.Models;
using backend_dotnet.Models.Dtos;

namespace backend_dotnet.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<ViewUser>> GetUserById(int id);
        Task<int> InsertUser(User user);
        Task UpdateUser(UpdateUser updateUser);
        Task DeleteUser(RemoveList removeList);
        Task<Object?> GetPages(Pager pager);
        Task<User> GetByUsernameAndPassword(string username, string pass);
    }
}