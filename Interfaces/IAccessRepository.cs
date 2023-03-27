using backend_dotnet.Models;

namespace backend_dotnet.Interfaces
{
    public interface IAccessRepository
    {
        Task<Object?> GetPages(Pager pager);
        Task<int> RegisterAccess(int id);
        Task FinishAccess(int id);
        Task DeleteAccess(RemoveList removeList);
    }
}