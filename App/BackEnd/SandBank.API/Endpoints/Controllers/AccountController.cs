using System;
using System.Threading.Tasks;
using Domain;
using Endpoints.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Endpoints.Controllers
{
    [Route("api/User/{id}/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SandBankDbContext _db;

        public AccountController(SandBankDbContext db) => _db = db;

        [HttpGet("{accountId}")]
        public async Task<IActionResult> Get([FromRoute] int id, [FromRoute] int accountId)
        {
            var account = await _db.Accounts
                .FirstOrDefaultAsync(acc => acc.AccountOwnerId == id && acc.Id == accountId);
            
            if (account != null)
            {
                return Ok(ToDisplayModel(account));
            }
            return NotFound();
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Account account, [FromRoute] int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            
            if (user == null)
            {
                return UnprocessableEntity();
            }

            account.AccountOwnerId = id;
            
            await _db.Accounts.AddAsync(account);
            await _db.SaveChangesAsync();
            
            return Ok(ToDisplayModel(account));
        }

        private Object ToDisplayModel(Account account)
        {
            return new
            {
                Id = account.Id,
                AccountType = account.AccountType,
                AccountNumber = account.AccountNumber,
                DisplayName = account.DisplayName
            };
        }
    }
}