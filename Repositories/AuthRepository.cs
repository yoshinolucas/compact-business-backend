
using backend_dotnet.Config;
using backend_dotnet.Interfaces;
using backend_dotnet.Models;
using backend_dotnet.Services;
using Dapper;
using Microsoft.Data.SqlClient;

namespace backend_dotnet.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IUserRepository _user;
        private readonly IConfiguration _cfg;
        private readonly string cs;
        public AuthRepository(IConfiguration cfg, IUserRepository user)
        {
            _user = user;
            _cfg = cfg;
            cs = _cfg.GetConnectionString("Conn")!;
        }
        public async Task<Object?> Login(LoginUser loginUser)
        {
            await using var conn = new SqlConnection(cs);
            var user = await _user.GetByUsernameAndPassword(loginUser.Username!, loginUser.Password!);
            if( user != null && user.Status != 3 ) {
                var token = TokenService.GenerateToken(user, 3);
                return new {
                    token = token,
                    user = new {
                        id = user.Id,
                        role = user.Role,
                    }
                };
            }
            return null;
        }
    }
}