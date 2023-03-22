using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_dotnet.Models
{
    public class Document
    {
        public int Id {get; set;}
        public string? Rg {get;set;}
        public string? Cpf { get; set; }
        public string? Pis {get;set;}
        public string? Ctps {get;set;}
        public string? Cnh {get;set;}
    }
}