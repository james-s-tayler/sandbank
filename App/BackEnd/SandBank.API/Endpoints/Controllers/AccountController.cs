using System;
using System.Threading.Tasks;
using Domain;
using Domain.Account;
using Endpoints.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Endpoints.Controllers
{
    [ApiController]
    [Route("api/User/{id}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AccountController : ControllerBase
    {
        private readonly SandBankDbContext _db;

        public AccountController(SandBankDbContext db) => _db = db;

        [HttpGet("{accountId}")]
        [ProducesResponseType(200, Type = typeof(AccountViewModel))]
        public async Task<IActionResult> Get([FromRoute] int id, [FromRoute] int accountId)
        {
            var account = await _db.Accounts
                .FirstOrDefaultAsync(acc => acc.AccountOwnerId == id && acc.Id == accountId);
            
            if (account != null)
            {
                return Ok(new AccountViewModel(account));
            }
            return NotFound();
        }
        
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(AccountViewModel))]
        public async Task<IActionResult> Post([FromBody] OpenAccountRequest openAccountRequest, [FromRoute] int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            
            if (user == null)
            {
                return UnprocessableEntity();
            }

            var account = openAccountRequest.ToDomainModel();
            account.AccountOwnerId = id;
            
            await _db.Accounts.AddAsync(account);
            await _db.SaveChangesAsync();
            
            return Ok(new AccountViewModel(account));
        }
    }
}