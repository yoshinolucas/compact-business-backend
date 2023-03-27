using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_dotnet.Models
{
    public class Filters
    {
        public int[]? status { get; set; }
        public string[]? teams { get; set; }
        public bool archived {get; set;}
        public int date {get;set;}
        public string[]? date_range {get;set;}
        public string? order {get;set;}
        public int? id {get;set;}
    }
}