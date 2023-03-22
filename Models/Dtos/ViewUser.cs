using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_dotnet.Models.Dtos
{
    public class ViewUser
    {
        public int Id {get;set;}
        public string? Nome {get; set;}
        public string? Sobrenome {get;set;}
        public string? Username {get;set;}
        public string? Email {get;set;}
        public string? Equipe {get;set;}
        public int Privilegio {get;set;}
        public int Status{get;set;}
        public int Arquivado{get;set;}
        public DateTime Created_at {get;set;}
        public DateTime? Updated_at {get;set;}
        public string? Data_nasc {get;set;}
        public string? Genero {get;set;}
        public int? EnderecoId {get;set;}
        public int? DocumentoId {get;set;}
    }
}