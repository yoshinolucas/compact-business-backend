using backend_dotnet.Models;

namespace backend_dotnet.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> All();
        Task<User> Get(int id);
        Task Create(User user);
        Task Update(UpdateUser updateUser);
        Task Remove(RemoveUser removeUser);
        Task<Object?> GetPage(Pager pager);
    }
}