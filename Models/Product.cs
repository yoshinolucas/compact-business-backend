namespace backend_dotnet.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Description_details {get;set;}
        public string Sku {get;set;}
        public decimal Price { get; set; }
        public string? Brand { get; set; }
        public string Measure { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public int? ComponentId { get; set; }
        public int? RawId {get;set;}
        public string? Obs { get; set; }
        public DateTime Created_at {get;set;}
        public DateTime? Updated_at { get; set; }
    }
}