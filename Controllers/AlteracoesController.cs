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
    public class AlteracoesController : ControllerBase
    {
        private readonly IAlteracoesRepository _altRepository;
        public AlteracoesController(IAlteracoesRepository altRepository)        
        {
            _altRepository = altRepository;
        }

        [Authorize]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(Alteracoes alteracao) {
            await _altRepository.Register(alteracao);
            return Ok();
        } 

        //Paginação
        [Authorize]
        [HttpPost]
        [Route("table")]
        public async Task<IActionResult> GetPages(Pager pager) {
            return Ok(await _altRepository.GetPages(pager));
        } 

        [Authorize]
        [HttpGet]
        [Route("id/{id}")]
        public async Task<IActionResult> GetAlteracaoById(int id){
            return Ok(await _altRepository.Id(id));
        }


    }
}