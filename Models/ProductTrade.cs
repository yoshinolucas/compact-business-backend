
namespace backend_dotnet.Models
{
    public class ProductTrade
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public long Amount { get; set; }
        public decimal Price { get; set; }
        public DateTime Trade_date { get; set; }
        public int Type {get;set;}
    }
}