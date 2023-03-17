
using backend_dotnet.Config;
using backend_dotnet.Interfaces;
using backend_dotnet.Models;
using backend_dotnet.Services;
using Dapper;

namespace backend_dotnet.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        public async Task<Object?> Login(LoginUser loginUser)
        {
            using var conn = ConfigGlobal.GetConnection();
            var result = await UserRepository.GetByUsernameAndPassword(loginUser.Username, loginUser.Password);
            if( result != null ) {
                var token = TokenService.GenerateToken(result, 3);
                return new {
                    token = token,
                    userId = result.Id
                };
            }
            return null;
        }

        public Task<bool> Logout(User user)
        {
            throw new NotImplementedException();
        }
    }
}