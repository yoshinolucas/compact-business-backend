using System.Text;
using backend_dotnet.Interfaces;
using Microsoft.Data.SqlClient;

namespace backend_dotnet.Config
{
    public static class ConfigGlobal
    {     
        public const String ConnectionString = @"
        Server=localhost;
        Database=prototype;
        User ID=sa;Password=root;
        Trusted_Connection=SSPI;
        Encrypt=false;
        TrustServerCertificate=true";

        public static String Secret = "qwertyuiopasdfghjklzxcvbnm123456";
        
        public static SqlConnection GetConnection() {
            return new SqlConnection(ConfigGlobal.ConnectionString);
        }
    }
}