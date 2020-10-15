using Chess.Api.Models;
using Chess.Api.Repositories.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace Chess.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MySqlConnection _connection;
        public UserRepository(IConfiguration configuration)
        {
            var dbConnectionString = configuration.GetValue<string>("DbConnectionString");
            _connection = new MySqlConnection(dbConnectionString);
        }

        public int CreateUser(string username, string passwordHash, string salt)
        {
            var parameters = new DynamicParameters();
            parameters.Add("usernameInput", username);
            parameters.Add("passwordHashInput", passwordHash);
            parameters.Add("passwordSaltInput", salt);
            parameters.Add("idOutput", direction: ParameterDirection.Output);

            _connection.Execute("CreateUser", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("idOutput");
        }

        public User GetUserById(int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("userIdInput", userId);

            return _connection.QueryFirstOrDefault<User>("GetUserById", parameters, commandType: CommandType.StoredProcedure);
        }

        public HashedCredentials GetUserCredentialsByUsername(string username)
        {
            var parameters = new DynamicParameters();
            parameters.Add("usernameInput", username);
            parameters.Add("idOutput", direction: ParameterDirection.Output);
            parameters.Add("passwordHashOutput", direction: ParameterDirection.Output);
            parameters.Add("passwordSaltOutput", direction: ParameterDirection.Output);

            _connection.Execute("GetUserCredentialsByUsername", parameters, commandType: CommandType.StoredProcedure);

            try
            {
                return new HashedCredentials
                {
                    UserId = parameters.Get<int>("idOutput"),
                    HashedPassword = parameters.Get<string>("passwordHashOutput"),
                    Salt = parameters.Get<string>("passwordSaltOutput")
                };
            } catch // Unable to get data - username does not exist
            {
                return null;
            }
        }
    }
}
