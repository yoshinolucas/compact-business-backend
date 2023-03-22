using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_dotnet.Models
{
    public class UpdateUser
    {
        public int[]? ids { get; set; }
        public string[]? columns {get;set;}
        public UpdateUserDto? user {get;set;}
    }

    public class UpdateUserDto {
        public string? Nome {get; set;}
        public string? Sobrenome {get;set;}
        public string? Username {get;set;}
        public string? Email {get;set;}
        public string? Equipe {get;set;}
        public int Privilegio {get;set;}
        public int Status{get;set;}
        public int Arquivado{get;set;}
        public string? Data_nasc {get;set;}
        public string? Salario {get;set;}
        public string? Genero {get;set;}
        public int? EnderecoId {get;set;}
        public int? DocumentoId {get;set;}
    }
}