using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_dotnet.Models
{
    public class Alteracoes
    {
        public int Id { get; set; }
        public int Section { get; set; }
        public int UserId {get;set;}
        public string? Antes {get;set;}
        public string? Depois {get;set;}
        public DateTime Data {get;set;}
        public int Action {get;set;}
    }
}