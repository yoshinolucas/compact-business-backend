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
        public string? Name {get; set;}
        public string? Lastname {get;set;}
        public string? Username {get;set;}
        public string? Email {get;set;}
        public string? Team {get;set;}
        public int Role {get;set;}
        public int Status{get;set;}
        public int Archived{get;set;}
        public string? Birthday_date {get;set;}
        public string? Salary {get;set;}
        public string? Gender {get;set;}
    }
}