using System.Net;
using System.Threading.Tasks;
using Core.Jwt;
using Core.MultiTenant;
using Database;
using Entities.System.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Endpoints.Controllers
{
    //would actually be nice to wire these in via something like Fody so that it's not even necessary to specify it
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class UserController : ControllerBase
    {
        private readonly SandBankDbContext _db;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<UserController> _logger;

        public UserController(SandBankDbContext db,
            IJwtTokenService jwtTokenService,
            ITenantProvider tenantProvider,
            ILogger<UserController> logger)
        {
            _db = db;
            _jwtTokenService = jwtTokenService;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserViewModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUser()
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == _tenantProvider.GetTenantId());
            
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

        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest loginUserRequest)
        {
            _logger.LogDebug($"Attempting to log in user: {loginUserRequest.Email}");
            
            var user = await _db.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Email == loginUserRequest.Email);
            //need to match password here too once password has been added
            
            if (user == null)
            { 
                return NotFound();
            }

            var jwtToken = _jwtTokenService.GenerateToken(user.Id);
            
            return Ok(jwtToken);
        }
    }
}