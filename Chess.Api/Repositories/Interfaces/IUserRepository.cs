namespace Chess.Api.Interfaces.Repositories
{
    public interface IUserRepository
    {
        int CreateUser(string username, string passwordHash, string salt);
    }
}
