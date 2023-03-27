using backend_dotnet.Interfaces;
using backend_dotnet.Models;
using backend_dotnet.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_dotnet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository repo)
        {
            _productRepository = repo;
        }


        [Authorize]
        [HttpPost]
        [Route("pages")]
        public async Task<IActionResult> Pages(Pager pager) => Ok(await _productRepository.GetPages(pager));    

        [Authorize]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(Product product) => Ok( await _productRepository.InsertProduct(product));

        
        [Authorize]
        [HttpPost]
        [Route("edit")]
        public async Task<IActionResult> Edit(UpdateProductDto updateProductDto) {
            await _productRepository.Update(updateProductDto);
            return Ok();
        } 

        [Authorize]
        [HttpGet]
        [Route("details/{id}")]
        public async Task<IActionResult> Details(int id) => Ok(await _productRepository.GetProductById(id));
         
        
        [Authorize]
        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete(RemoveList removeList) {
            await _productRepository.Delete(removeList);
            return Ok();
        } 

        [Authorize]
        [HttpPost]
        [Route("trades/register")]
        public async Task<IActionResult> RegisterIn(ProductTrade productTrade) {
            return Ok(await _productRepository.InsertProductTrade(productTrade));
        } 

        [Authorize]
        [HttpPost]
        [Route("trades/pages")]
        public async Task<IActionResult> TradesPages(Pager pager) {
            return Ok(await _productRepository.GetTradePages(pager));
        } 
        

    }
}