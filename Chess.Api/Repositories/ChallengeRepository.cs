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
        private readonly MySqlConnection _connection;
        public ChallengeRepository(IConfiguration configuration, ILogger<ChallengeRepository> logger)
        {
            var dbConnectionString = configuration.GetValue<string>("DbConnectionString");
            logger.LogInformation($"ChallengeRepository DB connection string: {dbConnectionString}");
            _connection = new MySqlConnection(dbConnectionString);
        }

        public IEnumerable<ChallengeDatabaseModel> GetChallengesByRecipient(int recipientId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("userIdInput", recipientId);

            return _connection.Query<ChallengeDatabaseModel>("GetChallengesByRecipient", parameters, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<ChallengeDatabaseModel> GetChallengesByChallenger(int challengerId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("userIdInput", challengerId);

            return _connection.Query<ChallengeDatabaseModel>("GetChallengesByChallenger", parameters, commandType: CommandType.StoredProcedure);
        }

        public ChallengeDatabaseModel GetChallenge(int challengerId, int recipientId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("challengerIdInput", challengerId);
            parameters.Add("recipientIdInput", recipientId);

            return _connection.QueryFirstOrDefault<ChallengeDatabaseModel>("GetChallenge", parameters,
                commandType: CommandType.StoredProcedure);
        }

        public void CreateChallenge(int challengerId, int recipientId, ChallengerColor challengerColor)
        {
            var parameters = new DynamicParameters();
            parameters.Add("challengerIdInput", challengerId);
            parameters.Add("recipientIdInput", recipientId);
            parameters.Add("challengerColorInput", challengerColor);

            _connection.Execute("CreateChallenge", parameters, commandType: CommandType.StoredProcedure);
        }

        public void DeleteChallenge(int challengerId, int recipientId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("challengerIdInput", challengerId);
            parameters.Add("recipientIdInput", recipientId);

            _connection.Execute("DeleteChallenge", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
