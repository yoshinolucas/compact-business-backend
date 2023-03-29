using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_dotnet.Models.Dtos
{
    public class ViewUser
    {
        public int Id {get;set;}
        public string? Name {get; set;}
        public string? Lastname {get;set;}
        public string? Username {get;set;}
        public string? Email {get;set;}
        public string? Team {get;set;}
        public int Role {get;set;}
        public int Status{get;set;}
        public int Archived{get;set;}
        public DateTime Created_at {get;set;}
        public DateTime? Updated_at {get;set;}
        public string? Birthday_date {get;set;}
        public string? Gender {get;set;}
        public Address Address {get;set;}
        public Document Document { get; set; }
    }
}