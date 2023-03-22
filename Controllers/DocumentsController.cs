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
         private readonly IDocumentRepository _documentRepository;

        public DocumentosController(IDocumentRepository repo)
        {
            _documentRepository = repo;
        }

        [Authorize]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(Document document) => Ok(await _documentRepository.InsertDocument(document));
        

        [Authorize]
        [HttpPost]
        [Route("edit/{id}")]
        public async Task<IActionResult> Edit(Document document, int id) {
            await _documentRepository.UpdateDocument(document, id);
            return Ok();
        } 

        [Authorize]
        [HttpGet]
        [Route("details/{id}")]
        public async Task<IActionResult> Details(int id) => Ok(await _documentRepository.GetDocumentById(id));
        
        
    }
}