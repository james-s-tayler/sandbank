using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Core.Jwt;
using Domain.User;
using Endpoints.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Endpoints.Controllers
{
    //would actually be nice to wire these in via something like Fody so that it's not even necessary to specify it
    [Authorize] 
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class UserController : ControllerBase
    {
        private readonly SandBankDbContext _db;
        private readonly IJwtTokenService _jwtTokenService;

        public UserController(SandBankDbContext db, IJwtTokenService jwtTokenService)
        {
            _db = db;
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserViewModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            
            if (user != null)
            {
                return Ok(new UserViewModel(user));
            }
            return NotFound();
        }
        
        
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserViewModel))]
        public async Task<IActionResult> PostUser([FromBody] RegisterUserRequest registerUserRequest)
        {
            var user = await _db.Users.AddAsync(registerUserRequest.ToDomainModel());
            await _db.SaveChangesAsync();
            
            return Ok(new UserViewModel(user.Entity));
        }

        public class LoginUserRequest 
        {
            public string Email { get; set; }
            
            // public string Password { get; set; }
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest loginUserRequest)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == loginUserRequest.Email);
            if (user == null)
            {
                return NotFound();
            }

            var jwtToken = _jwtTokenService.GenerateToken(user.Id);
            
            return Ok(jwtToken);
        }
    }
}