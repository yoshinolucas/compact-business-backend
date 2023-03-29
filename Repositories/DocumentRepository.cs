using backend_dotnet.Config;
using backend_dotnet.Interfaces;
using backend_dotnet.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace backend_dotnet.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IConfiguration _cfg;
        private readonly string cs;
        public DocumentRepository(IConfiguration cfg)
        {
            _cfg = cfg;
            cs = _cfg.GetConnectionString("Conn")!;
        }
        public async Task<int> InsertDocument(Document document)
        {
            await using var conn = new SqlConnection(cs);
            return conn.QuerySingle<int>(@"INSERT INTO documents 
            (rg,cpf,cnh,pis,ctps,userId) 
            OUTPUT INSERTED.[id]
            VALUES 
            (@Rg,@Cpf,@Cnh,@Pis,@Ctps,@UserId)", document);
        }

        public async Task<Document> GetDocumentById(int id)
        {
            await using var conn = new SqlConnection(cs);
            return conn.QuerySingle<Document>(@"SELECT * FROM documents WHERE id = @Id", new { Id = id}); 
        }

        public async Task UpdateDocument(Document document, int id)
        {
            await using var conn = new SqlConnection(cs);
            await conn.ExecuteAsync(@"UPDATE documents SET
            cpf = @Cpf,
            rg = @Rg,
            pis = @Pis,
            ctps = @Ctps,
            cnh = @Cnh
            WHERE id = @Id", 
            new {
                Id = id,
                Cpf = document.Cpf,
                Rg = document.Rg,
                Pis = document.Pis,
                Ctps = document.Ctps,
                Cnh = document.Cnh,
            }); 
        }
    }
}