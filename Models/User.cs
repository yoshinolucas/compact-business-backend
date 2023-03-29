using System.Text.Json.Serialization;
using backend_dotnet.Interfaces;

namespace backend_dotnet.Models
{
    public class User
    {
        public long Id {get;set;}

        public string? Name {get;set;}

        public string? Lastname {get;set;}
        public string? Username {get;set;}
        public string? Password {get;set;}
        public string? Email {get;set;}
        public int Status { get; set; }
        public int Role { get; set; }
        public string? Team { get; set; }
        public string? Gender {get;set;}
        public string? Birthday_date { get; set; }
        public string? Salary {get;set;}
        public int Archived {get;set;}
        public DateTime Created_at {get;set;}
        public DateTime? Updated_at {get;set;}
        public Address? Address {get;set;}
        public Document? Document {get;set;}

   
    }
}