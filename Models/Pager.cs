namespace backend_dotnet.Models
{
    public class Pager
    {
        public int currentPage {get;set;}
        public int maxItems {get;set;}
        public string search {get;set;} = "";
        public Filters? filters { get; set; }
    }
}