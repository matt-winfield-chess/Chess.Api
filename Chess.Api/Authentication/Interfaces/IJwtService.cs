namespace Chess.Api.Authentication.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(int userId);
    }
}
