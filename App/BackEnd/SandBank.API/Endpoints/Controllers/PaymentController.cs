using System.Threading.Tasks;
using Endpoints.Data;
using Microsoft.AspNetCore.Mvc;

namespace Endpoints.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class PaymentController : ControllerBase
    {
        private readonly SandBankDbContext _db;

        public PaymentController(SandBankDbContext db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> PostPayment()
        {
            /*
             * post payment request
             *  -> toAcc, fromAcc, amount
             *
             *  if(valid && allowed)
             *  {
             *     create transaction debiting fromAccount
             *
             *     if(intrabank)
             *     {
             *         create transaction crediting toAccount
             *     }
             *     else
             *     {
             *         addToBatch(payment);
             *     }
             *     return Ok
             *  }
             * 
             *  return UnprocessableEntity();
             */
            
            return UnprocessableEntity();
        }
    }
}