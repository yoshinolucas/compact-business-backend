using backend_dotnet.Models;
using backend_dotnet.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace backend_dotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository repo)
        {
            _userRepository = repo;
        }

        [Authorize]
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> All() {
            return Ok(await _userRepository.All());
        }


        [Authorize]
        [HttpGet]
        [Route("id/{id}")]
        public async Task<IActionResult> Id(int id) {
            return Ok(await _userRepository.Get(id));
        }

        [Authorize]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(User user) {
            await _userRepository.Create(user);
            return Ok();
        } 

        [Authorize]
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(UpdateUser updateUser) {
            await _userRepository.Update(updateUser);
            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("remove")]
        public async Task<IActionResult> Remove(RemoveUser removeUser) {
            
            await _userRepository.Remove(removeUser);
            return Ok();
        }

        //Paginação
        [Authorize]
        [HttpPost]
        [Route("table")]
        public async Task<IActionResult> GetPage(Pager pager) {
            return Ok(await _userRepository.GetPage(pager));
        } 
    }
}