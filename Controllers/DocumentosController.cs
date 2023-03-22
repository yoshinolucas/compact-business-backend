using backend_dotnet.Interfaces;
using backend_dotnet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_dotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentosController : ControllerBase
    {
         private readonly IDocumentosRepository _documentosRepository;

        public DocumentosController(IDocumentosRepository repo)
        {
            _documentosRepository = repo;
        }

        [Authorize]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(Documentos documentos) {
            var newdocumentosId = await _documentosRepository.Create(documentos);
            return Ok(newdocumentosId);
        } 

        [Authorize]
        [HttpPost]
        [Route("update/{id}")]
        public async Task<IActionResult> Update(Documentos documentos, int id) {
            await _documentosRepository.Update(documentos, id);
            return Ok();
        } 

        [Authorize]
        [HttpGet]
        [Route("id/{id}")]
        public async Task<IActionResult> Id(int id) {
            return Ok(await _documentosRepository.Get(id));
        }
        
    }
}