using System.Text.Json.Serialization;
using backend_dotnet.Interfaces;

namespace backend_dotnet.Models
{
    public class User
    {
        public long Id {get;set;}

        public string? Nome {get;set;}

        public string? Sobrenome {get;set;}
        public string? Username {get;set;}
        public string? Password {get;set;}

        public string? Email {get;set;}
        public int Status { get; set; }
        public int Privilegio { get; set; }
        public string? Equipe { get; set; }
        public string? Genero {get;set;}
        public string? Data_nasc { get; set; }
        public string? Salario {get;set;}
        public int Arquivado {get;set;}
        public DateTime Created_at {get;set;}
        public DateTime? Updated_at {get;set;}

        public int? EnderecoId { get; set; }
        public Endereco? Endereco {get;set;}

        public int? DocumentoId { get; set; }
        public Documentos? Documentos {get;set;}

   
    }
}