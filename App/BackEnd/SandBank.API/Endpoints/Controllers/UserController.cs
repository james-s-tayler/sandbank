using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Endpoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly List<User> _users = new List<User>();

        [HttpGet("hello")]
        public IActionResult Hello()
        {
            return Ok("hello world");
        }
        
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (!_users.Any())
            {
                var newUser = new User
                {
                    Id = 1,
                    FullName = "Solo Yolo",
                    Email = "solo@yolo.com",
                    Phone = "1234567",
                    Address = "123 Fake St, Auckland",
                    City = "Auckland",
                    DateOfBirth = DateTime.Now.AddYears(-31)
                };
                
                _users.Add(newUser);
            }
            
            var user = _users.FirstOrDefault(u => u.Id == id);
            
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }
        
        [HttpPost]
        public IActionResult Post([FromBody] User user)
        {
            _users.Add(user);
            
            return Ok(user);
        }
    }
}