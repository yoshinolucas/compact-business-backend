using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend_dotnet.Interfaces;
using backend_dotnet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_dotnet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessesController : ControllerBase
    {
        private readonly IAccessRepository _accessRepository;

        public AccessesController(IAccessRepository repo)
        {
            _accessRepository = repo;
        }

        [Authorize]
        [HttpPost]
        [Route("register/{id}")]
        public async Task<IActionResult> Register(int id) => Ok(await _accessRepository.RegisterAccess(id));

        [Authorize]
        [HttpPost]
        [Route("finish/{id}")]
        public async Task<IActionResult> Finish(int id) {
            await _accessRepository.FinishAccess(id);
            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("pages")]
        public async Task<IActionResult> Pages(Pager pager) => Ok(await _accessRepository.GetPages(pager));    

        [Authorize]
        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete(RemoveList removeList) {
            await _accessRepository.DeleteAccess(removeList);
            return Ok(); 
        }     

    }
}