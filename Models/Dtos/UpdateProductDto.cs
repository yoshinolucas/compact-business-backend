using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_dotnet.Models.Dtos
{
    public class UpdateProductDto
    {
        public IEnumerable<int>? Ids {get;set;}
        public IEnumerable<string>? Columns {get;set;}
        public Product? Product {get;set;}
    }
}