using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_dotnet.Models
{
    public class UpdateUser
    {
        public int[]? ids { get; set; }
        public User? user { get; set; }
        public string[]? columns {get;set;}
    }
}