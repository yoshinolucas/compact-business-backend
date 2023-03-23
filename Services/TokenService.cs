using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend_dotnet.Config;
using backend_dotnet.Models;
using Microsoft.IdentityModel.Tokens;

namespace backend_dotnet.Services
{
    public class TokenService
    {
        public static string GenerateToken(User user, int hours){
            var key = Encoding.ASCII.GetBytes(ConfigGlobal.Secret);
            var tokenConfig = new SecurityTokenDescriptor{
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]{
                    new Claim("id", user.Id.ToString()),
                }), 
                Expires = DateTime.UtcNow.AddHours(hours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenConfig);
            return tokenHandler.WriteToken(token);          
        }
    }
}