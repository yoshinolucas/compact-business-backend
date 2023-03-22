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
    [Route("api/[controller]")]
    [ApiController]
    public class EnderecosController : ControllerBase
    {
         private readonly IEnderecoRepository _enderecoRepository;

        public EnderecosController(IEnderecoRepository repo)
        {
            _enderecoRepository = repo;
        }

        [Authorize]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(Endereco endereco) {
            var newEnderecoId = await _enderecoRepository.Create(endereco);
            return Ok(newEnderecoId);
        } 

        [Authorize]
        [HttpPost]
        [Route("update/{id}")]
        public async Task<IActionResult> Update(Endereco endereco, int id) {
            await _enderecoRepository.Update(endereco, id);
            return Ok();
        } 

        [Authorize]
        [HttpGet]
        [Route("id/{id}")]
        public async Task<IActionResult> Id(int id) {
            return Ok(await _enderecoRepository.Get(id));
        }
        
    }
}