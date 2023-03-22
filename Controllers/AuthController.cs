

using backend_dotnet.Interfaces;
using backend_dotnet.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend_dotnet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository repo)
        {
            _authRepository = repo;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginUser loginUser){
            var success = await _authRepository.Login(loginUser);
            if ( success != null ) return Ok(success);
            return NotFound();
        } 
    }
}