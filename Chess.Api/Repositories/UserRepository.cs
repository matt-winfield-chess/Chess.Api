using Chess.Api.Models;
using Chess.Api.Repositories.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Data;

namespace Chess.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        public UserRepository(IConfiguration configuration, ILogger<UserRepository> logger)
        {
            var dbConnectionString = configuration.GetValue<string>("DbConnectionString");
            logger.LogInformation($"UserRepository DB connection string: {dbConnectionString}");
            _connectionString = dbConnectionString;
        }

        public bool CheckHealth()
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            var isSuccess = connection.State == ConnectionState.Open;
            connection.Close();
            return isSuccess;
        }

        public int CreateUser(string username, string passwordHash, string salt)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("usernameInput", username);
            parameters.Add("passwordHashInput", passwordHash);
            parameters.Add("passwordSaltInput", salt);
            parameters.Add("idOutput", direction: ParameterDirection.Output);

            connection.Execute("CreateUser", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("idOutput");
        }

        public User GetUserById(int userId)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("userIdInput", userId);

            return connection.QueryFirstOrDefault<User>("GetUserById", parameters, commandType: CommandType.StoredProcedure);
        }

        public int GetUserPieceMovementMethod(int userId)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("userIdInput", userId);

            return connection.QueryFirstOrDefault<int>("GetUserPieceMovementMethod", parameters,
                commandType: CommandType.StoredProcedure);
        }

        public void SetUserPieceMovementMethod(int userId, int movementMethod)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("userIdInput", userId);
            parameters.Add("pieceMovementMethodInput", movementMethod);

            connection.Execute("SetUserPieceMovementMethod", parameters,
                commandType: CommandType.StoredProcedure);
        }

        public HashedCredentials GetUserCredentialsByUsername(string username)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("usernameInput", username);
            parameters.Add("idOutput", direction: ParameterDirection.Output);
            parameters.Add("passwordHashOutput", direction: ParameterDirection.Output);
            parameters.Add("passwordSaltOutput", direction: ParameterDirection.Output);

            connection.Execute("GetUserCredentialsByUsername", parameters, commandType: CommandType.StoredProcedure);

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
