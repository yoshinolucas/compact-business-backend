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
        [Route("details/{id}")]
        public async Task<IActionResult> Details(int id) => Ok(await _userRepository.GetUserById(id));
        

        [Authorize]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(User user) => Ok(await _userRepository.InsertUser(user));
        

        [Authorize]
        [HttpPost]
        [Route("edit")]
        public async Task<IActionResult> Edit(UpdateUser updateUser) {
            await _userRepository.UpdateUser(updateUser);
            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete(RemoveList removeList) {
            
            await _userRepository.DeleteUser(removeList);
            return Ok();
        }

        //Paginação
        [Authorize]
        [HttpPost]
        [Route("pages")]
        public async Task<IActionResult> Pages(Pager pager) => Ok(await _userRepository.GetPages(pager));
        
    }
}