using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_dotnet.Models
{
    public class Documentos
    {
        public int Id {get; set;}
        public string? Rg {get;set;}
        public string? Cpf { get; set; }
        public string? Pis {get;set;}
        public string? Carteira_trabalho {get;set;}
        public string? Cnh {get;set;}
    }
}