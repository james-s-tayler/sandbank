using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Domain.User;
using Endpoints.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Endpoints.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class UserController : ControllerBase
    {
        private readonly SandBankDbContext _db;

        public UserController(SandBankDbContext db) => _db = db;

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            
            if (user != null)
            {
                return Ok(new UserViewModel(user));
            }
            return NotFound();
        }
        
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        public async Task<IActionResult> PostUser([FromBody] RegisterUserRequest registerUserRequest)
        {
            var user = await _db.Users.AddAsync(registerUserRequest.ToDomainModel());
            await _db.SaveChangesAsync();
            
            return Ok(new UserViewModel(user.Entity));
        }

        public class LoginUserRequest 
        {
            public string Email {get; set; }
            
            // public string Password { get; set; }
        }

        [HttpPost("Login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest loginUserRequest)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == loginUserRequest.Email);
            if (user == null)
            {
                return NotFound();

            }

            return Ok();
        }
    }
}