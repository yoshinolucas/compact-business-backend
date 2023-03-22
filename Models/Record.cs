using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_dotnet.Models
{
    public class Record
    {
        public int Id { get; set; }
        public int Section { get; set; }
        public int UserId {get;set;}
        public string? Before {get;set;}
        public string? After {get;set;}
        public DateTime Registered_at {get;set;}
        public int Action {get;set;}
    }
}