using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend_dotnet.Models;

namespace backend_dotnet.Interfaces
{
    public interface IEnderecoRepository
    {
        Task<Endereco> Get(int id);
        Task<int> Create(Endereco endereco);
        Task Update(Endereco endereco, int id);
    }
}