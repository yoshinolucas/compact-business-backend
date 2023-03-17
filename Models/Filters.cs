using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_dotnet.Models
{
    public class Filters
    {
        public int[]? status { get; set; }
        public string[]? equipe { get; set; }
        public bool arquivado {get; set;}
    }
}