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
    public class GameRepository : IGameRepository
    {
        private readonly MySqlConnection _connection;
        public GameRepository(IConfiguration configuration, ILogger<GameRepository> logger)
        {
            var dbConnectionString = configuration.GetValue<string>("DbConnectionString");
            logger.LogInformation($"GameRepository DB connection string: {dbConnectionString}");
            _connection = new MySqlConnection(dbConnectionString);
        }

        public void CreateGame(string gameId, int whitePlayerId, int blackPlayerId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("idInput", gameId);
            parameters.Add("whitePlayerIdInput", whitePlayerId);
            parameters.Add("blackPlayerIdInput", blackPlayerId);

            _connection.Execute("CreateGame", parameters, commandType: CommandType.StoredProcedure);
        }

        public GameDatabaseModel GetGameById(string gameId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("idInput", gameId);

            return _connection.QueryFirstOrDefault<GameDatabaseModel>("GetGameById", parameters, commandType: CommandType.StoredProcedure);
        }

        public void AddMoveToGame(string gameId, string move)
        {
            var parameters = new DynamicParameters();
            parameters.Add("gameIdInput", gameId);
            parameters.Add("moveInput", move);

            _connection.Execute("AddMoveToGame", parameters, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<MoveDatabaseModel> GetMovesByGameId(string gameId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("gameIdInput", gameId);

            return _connection.Query<MoveDatabaseModel>("GetGameMovesById", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
