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
    public class AddressesController : ControllerBase
    {
        private readonly IAddressRepository _addressRepository;

        public AddressesController(IAddressRepository repo)
        {
            _addressRepository = repo;
        }

        [Authorize]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(Address Address) => Ok(await _addressRepository.InsertAddress(Address));
        

        [Authorize]
        [HttpPost]
        [Route("edit/{id}")]
        public async Task<IActionResult> Edit(Address Address, int id) {
            await _addressRepository.UpdateAddress(Address, id);
            return Ok();
        } 

        [Authorize]
        [HttpGet]
        [Route("details/{id}")]
        public async Task<IActionResult> Details(int id) => Ok(await _addressRepository.GetAddressById(id));
        
        
    }
}