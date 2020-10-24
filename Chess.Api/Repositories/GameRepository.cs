using Chess.Api.Models.Database;
using Chess.Api.Repositories.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace Chess.Api.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly string _connectionString;
        public GameRepository(IConfiguration configuration, ILogger<GameRepository> logger)
        {
            var dbConnectionString = configuration.GetValue<string>("DbConnectionString");
            logger.LogInformation($"GameRepository DB connection string: {dbConnectionString}");
            _connectionString = dbConnectionString;
        }

        public void CreateGame(string gameId, int whitePlayerId, int blackPlayerId)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("idInput", gameId);
            parameters.Add("whitePlayerIdInput", whitePlayerId);
            parameters.Add("blackPlayerIdInput", blackPlayerId);

            connection.Execute("CreateGame", parameters, commandType: CommandType.StoredProcedure);
        }

        public GameDatabaseModel GetGameById(string gameId)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("idInput", gameId);

            return connection.QueryFirstOrDefault<GameDatabaseModel>("GetGameById", parameters, commandType: CommandType.StoredProcedure);
        }

        public void AddMoveToGame(string gameId, string move)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("gameIdInput", gameId);
            parameters.Add("moveInput", move);

            connection.Execute("AddMoveToGame", parameters, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<MoveDatabaseModel> GetMovesByGameId(string gameId)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("gameIdInput", gameId);

            return connection.Query<MoveDatabaseModel>("GetGameMovesById", parameters, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<GameDatabaseModel> GetUserActiveGames(int userId)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("userIdInput", userId);

            return connection.Query<GameDatabaseModel>("GetActiveGames", parameters,
                commandType: CommandType.StoredProcedure);
        }
    }
}
