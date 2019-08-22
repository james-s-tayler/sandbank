using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Endpoints.Data;
using Microsoft.AspNetCore.Mvc;

namespace Endpoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SandBankDbContext _db;

        public UserController(SandBankDbContext db) => _db = db;

        [HttpGet("hello")]
        public IActionResult Hello()
        {
            return Ok("hello world");
        }
        
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (!_db.Users.Any())
            {
                var newUser = new User
                {
                    FullName = "Solo Yolo",
                    Email = "solo@yolo.com",
                    Phone = "1234567",
                    Address = "123 Fake St, Auckland",
                    City = "Auckland",
                    DateOfBirth = DateTime.Now.AddYears(-31)
                };
                
                _db.Users.Add(newUser);
                _db.SaveChanges();
            }
            
            var user = _db.Users.FirstOrDefault(u => u.Id == id);
            
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }
        
        [HttpPost]
        public IActionResult Post([FromBody] User user)
        {
            _db.Users.Add(user);
            _db.SaveChanges();
            
            return Ok(user);
        }
    }
}