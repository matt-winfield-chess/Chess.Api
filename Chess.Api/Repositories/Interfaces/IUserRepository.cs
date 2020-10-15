﻿using Chess.Api.Models;

namespace Chess.Api.Repositories.Interfaces
{
    public interface IUserRepository
    {
        int CreateUser(string username, string passwordHash, string salt);
        HashedCredentials GetUserCredentialsByUsername(string username);
        User GetUserById(int userId);
    }
}
