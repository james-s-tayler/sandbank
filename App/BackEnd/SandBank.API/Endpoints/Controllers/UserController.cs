using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            
            return Ok(user);
        }
    }
}