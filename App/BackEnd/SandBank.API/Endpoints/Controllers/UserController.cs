using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Domain;
using Endpoints.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Endpoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SandBankDbContext _db;

        public UserController(SandBankDbContext db) => _db = db;

        [HttpGet("{id}")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(UserDisplayDTO))]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            
            if (user != null)
            {
                return Ok(new UserDisplayDTO(user));
            }
            return NotFound();
        }
        
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(200, Type = typeof(UserDisplayDTO))]
        public async Task<IActionResult> Post([FromBody] UserRegisterDTO userDto)
        {
            var user = await _db.Users.AddAsync(userDto.ToUser());
            await _db.SaveChangesAsync();
            
            return Ok(new UserDisplayDTO(user.Entity));
        }
    }
}