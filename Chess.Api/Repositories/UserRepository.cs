using Chess.Api.Interfaces.Repositories;
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
    }
}
