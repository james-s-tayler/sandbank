namespace Core.Jwt
{
    public interface IJwtTokenService
    {
        string GenerateToken(int email);
    }
}