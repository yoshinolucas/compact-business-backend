namespace backend_dotnet.Models
{
    public class Access
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Login_at {get;set;}
        public DateTime? Logout_at { get; set; }
    }
}