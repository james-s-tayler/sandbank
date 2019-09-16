using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Core.MultiTenant
{
    public class TenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TenantProvider> _logger;

        public TenantProvider(IHttpContextAccessor contextAccessor, ILogger<TenantProvider> logger)
        {
            _httpContextAccessor = contextAccessor;
            _logger = logger;
        }

        //violates the shit out of the law of demeter
        //would blow up if ever called via a method that allowed anonymous access
        public int GetTenantId()
        {
            var user = _httpContextAccessor.HttpContext.User;
            var tenantId = user.FindFirst(ClaimTypes.NameIdentifier);

            return int.Parse(tenantId.Value);
        }
    }
}