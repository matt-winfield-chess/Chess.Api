using Chess.Api.Models;
using Chess.Api.Repositories.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using Chess.Api.Models.Database;

namespace Chess.Api.Repositories
{
    public class ChallengeRepository : IChallengeRepository
    {
        private readonly string _connectionString;
        public ChallengeRepository(IConfiguration configuration, ILogger<ChallengeRepository> logger)
        {
            var dbConnectionString = configuration.GetValue<string>("DbConnectionString");
            logger.LogInformation($"ChallengeRepository DB connection string: {dbConnectionString}");
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

        public IEnumerable<ChallengeDatabaseModel> GetChallengesByRecipient(int recipientId)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("userIdInput", recipientId);

            return connection.Query<ChallengeDatabaseModel>("GetChallengesByRecipient", parameters, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<ChallengeDatabaseModel> GetChallengesByChallenger(int challengerId)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("userIdInput", challengerId);

            return connection.Query<ChallengeDatabaseModel>("GetChallengesByChallenger", parameters, commandType: CommandType.StoredProcedure);
        }

        public ChallengeDatabaseModel GetChallenge(int challengerId, int recipientId)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("challengerIdInput", challengerId);
            parameters.Add("recipientIdInput", recipientId);

            return connection.QueryFirstOrDefault<ChallengeDatabaseModel>("GetChallenge", parameters,
                commandType: CommandType.StoredProcedure);
        }

        public void CreateChallenge(int challengerId, int recipientId, ChallengerColor challengerColor)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("challengerIdInput", challengerId);
            parameters.Add("recipientIdInput", recipientId);
            parameters.Add("challengerColorInput", challengerColor);

            connection.Execute("CreateChallenge", parameters, commandType: CommandType.StoredProcedure);
        }

        public void DeleteChallenge(int challengerId, int recipientId)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("challengerIdInput", challengerId);
            parameters.Add("recipientIdInput", recipientId);

            connection.Execute("DeleteChallenge", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
