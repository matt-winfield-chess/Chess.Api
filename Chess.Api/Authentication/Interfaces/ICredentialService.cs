namespace Chess.Api.Authentication.Interfaces
{
    public interface ICredentialService
    {
        byte[] GenerateRandomSalt();

        string HashPassword(string password, byte[] salt);
    }
}
