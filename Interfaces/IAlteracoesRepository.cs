using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend_dotnet.Models;

namespace backend_dotnet.Interfaces
{
    public interface IAlteracoesRepository
    {
        
        Task Register(Alteracoes alteracao);
        Task<Alteracoes> Id(int id);
        Task<Object?> GetPages(Pager pager);
    }
}