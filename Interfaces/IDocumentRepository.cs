using backend_dotnet.Models;

namespace backend_dotnet.Interfaces
{
    public interface IDocumentRepository
    {
        Task<Document> GetDocumentById(int id);
        Task<int> InsertDocument(Document document);
        Task UpdateDocument(Document document, int id);
    }
}