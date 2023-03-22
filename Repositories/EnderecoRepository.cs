using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend_dotnet.Config;
using backend_dotnet.Interfaces;
using backend_dotnet.Models;
using Dapper;

namespace backend_dotnet.Repositories
{
    public class EnderecoRepository : IEnderecoRepository
    {
        public async Task<int> Create(Endereco endereco)
        {
            await using var conn = ConfigGlobal.GetConnection(); 
            int newEnderecoId = conn.QuerySingle<int>(@"INSERT INTO enderecos 
            (rua,cep,estado,cidade,numero,bairro,pais) 
            OUTPUT INSERTED.[id]
            VALUES 
            (@Rua,@Cep,@Estado,@Cidade,@Numero,@Bairro,@Pais)", endereco);

            return newEnderecoId;            
        }

        public async Task<Endereco> Get(int id)
        {
            await using var conn = ConfigGlobal.GetConnection(); 
            var endereco = conn.QuerySingle<Endereco>(@"SELECT * FROM enderecos WHERE id = @Id", new { Id = id});

            return endereco; 
        }

        public async Task Update(Endereco endereco, int id)
        {
            await using var conn = ConfigGlobal.GetConnection(); 
            await conn.ExecuteAsync(@"UPDATE enderecos SET
            rua = @Rua,
            cep = @Cep,
            estado = @Estado,
            cidade = @Cidade,
            numero = @Numero,
            bairro = @Bairro,
            pais = @Pais
            WHERE id = @Id", 
            new {
                Id = id,
                Rua = endereco.Rua,
                Cep = endereco.Cep,
                Estado = endereco.Estado,
                Cidade = endereco.Cidade,
                Numero = endereco.Numero,
                Bairro = endereco.Bairro,
                Pais = endereco.Pais
            }); 
        }
    }
}