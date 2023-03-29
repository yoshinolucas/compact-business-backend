namespace backend_dotnet.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string? Street { get; set; }
        public string? Zip_code {get; set;}
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Number {get;set;}
        public string? District { get; set; }
        public string? Country {get;set;}
        public int UserId { get; set; }
    }
}