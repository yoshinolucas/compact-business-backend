using backend_dotnet.Models;

namespace backend_dotnet.Interfaces
{
    public interface IDocumentosRepository
    {
        Task<Documentos> Get(int id);
        Task<int> Create(Documentos documentos);
        Task Update(Documentos documentos, int id);
    }
}