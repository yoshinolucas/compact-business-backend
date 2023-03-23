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
    public class RecordsController : ControllerBase
    {
        private readonly IRecordRepository _recRepository;
        public RecordsController(IRecordRepository recRepository)        
        {
            _recRepository = recRepository;
        }

        [Authorize]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(Record rec) {
            await _recRepository.RegisterRecord(rec);
            return Ok();
        } 

        //Paginação
        [Authorize]
        [HttpPost]
        [Route("pages")]
        public async Task<IActionResult> Pages(Pager pager) => Ok(await _recRepository.GetPages(pager));
        

        [Authorize]
        [HttpGet]
        [Route("details/{id}")]
        public async Task<IActionResult> Details(int id) => Ok(await _recRepository.GetRecordById(id));
        


    }
}