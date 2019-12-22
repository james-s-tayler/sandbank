namespace Core.MultiTenant
{
    public interface ITenantProvider
    {
        int GetTenantId();
        void SetTenantId(int userId);
    }
}