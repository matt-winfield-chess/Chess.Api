namespace Chess.Api.Authentication
{
    public interface ICredentialService
    {
        byte[] GenerateRandomSalt();

        string HashPassword(string password, byte[] salt);
    }
}
