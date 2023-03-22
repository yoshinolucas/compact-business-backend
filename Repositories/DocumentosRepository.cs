using backend_dotnet.Config;
using backend_dotnet.Interfaces;
using backend_dotnet.Models;
using Dapper;

namespace backend_dotnet.Repositories
{
    public class DocumentosRepository : IDocumentosRepository
    {
        public async Task<int> Create(Documentos documentos)
        {
            await using var conn = ConfigGlobal.GetConnection(); 
            int newDocumentoId = conn.QuerySingle<int>(@"INSERT INTO documentos 
            (rg,cpf,cnh,pis,carteira_trabalho) 
            OUTPUT INSERTED.[id]
            VALUES 
            (@Rg,@Cpf,@Cnh,@Pis,@Carteira_trabalho)", documentos);

            return newDocumentoId;   
        }

        public async Task<Documentos> Get(int id)
        {
            await using var conn = ConfigGlobal.GetConnection(); 
            var documentos = conn.QuerySingle<Documentos>(@"SELECT * FROM documentos WHERE id = @Id", new { Id = id});
            return documentos;
        }

        public async Task Update(Documentos documentos, int id)
        {
            await using var conn = ConfigGlobal.GetConnection(); 
            await conn.ExecuteAsync(@"UPDATE documentos SET
            cpf = @Cpf,
            rg = @Rg,
            pis = @Pis,
            carteira_trabalho = @Carteira_trabalho,
            cnh = @Cnh
            WHERE id = @Id", 
            new {
                Id = id,
                Cpf = documentos.Cpf,
                Rg = documentos.Rg,
                Pis = documentos.Pis,
                Carteira_trabalho = documentos.Carteira_trabalho,
                Cnh = documentos.Cnh,
            }); 
        }
    }
}