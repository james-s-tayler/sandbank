using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Core.MultiTenant
{
    public class TenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TenantProvider> _logger;
        private int? TenantId { get; set; }

        public TenantProvider(IHttpContextAccessor contextAccessor, ILogger<TenantProvider> logger)
        {
            _httpContextAccessor = contextAccessor;
            _logger = logger;
        }
        
        public int GetTenantId()
        {
            var user = _httpContextAccessor?.HttpContext?.User;

            if (user == null && !TenantId.HasValue)
            {
                var errorMessage = "TenantId requested but no tenant is currently in context";
                var ex = new ApplicationException(errorMessage);
                _logger.LogError(errorMessage, ex);
                throw ex;
            }

            if (user != null)
            {
                var tenantId = user.FindFirst(ClaimTypes.NameIdentifier);
                return int.Parse(tenantId.Value);
            }

            return TenantId.Value;
        }

        public void SetTenantId(int userId)
        {
            TenantId = userId;
        }
    }
}