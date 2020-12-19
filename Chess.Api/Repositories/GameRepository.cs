using Chess.Api.Constants;
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
            CreateGame(gameId, whitePlayerId, blackPlayerId, GameConstants.STANDARD_START_POSITION_FEN);
        }

        public void CreateGame(string gameId, int whitePlayerId, int blackPlayerId, string fen)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("idInput", gameId);
            parameters.Add("whitePlayerIdInput", whitePlayerId);
            parameters.Add("blackPlayerIdInput", blackPlayerId);
            parameters.Add("fenInput", fen);

            connection.Execute("CreateGame", parameters, commandType: CommandType.StoredProcedure);
        }

        public GameDatabaseModel GetGameById(string gameId)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("idInput", gameId);

            return connection.QueryFirstOrDefault<GameDatabaseModel>("GetGameById", parameters, commandType: CommandType.StoredProcedure);
        }

        public void AddMoveToGame(string gameId, string move, string newFen)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("gameIdInput", gameId);
            parameters.Add("moveInput", move);
            parameters.Add("newFenInput", newFen);

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

        public void SetGameResult(string gameId, string winnerColor, int? winnerId, string termination)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("gameIdInput", gameId);
            parameters.Add("winnerColorInput", winnerColor);
            parameters.Add("winnerIdInput", winnerId);
            parameters.Add("terminationInput", termination);

            connection.Execute("SetGameResult", parameters, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<PositionDatabaseModel> GetPositionsSinceIrreversibleMove(string gameId)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("gameIdInput", gameId);

            return connection.Query<PositionDatabaseModel>("GetPositionsFromIrreversibleMove", parameters,
                commandType: CommandType.StoredProcedure);
        }

        public void AddPositionToGame(string gameId, string fen)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("gameIdInput", gameId);
            parameters.Add("fenInput", fen);

            connection.Execute("AddPositionToGame", parameters, commandType: CommandType.StoredProcedure);
        }

        public void ClearPositionsFromIrreversibleMove(string gameId)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("gameIdInput", gameId);

            connection.Execute("ClearGamePositionsFromIrreversibleMove", parameters, commandType: CommandType.StoredProcedure);
        }

        public void CreateDrawOffer(string gameId, string color)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("gameIdInput", gameId);
            parameters.Add("drawOfferColor", color);

            connection.Execute("OfferDraw", parameters, commandType: CommandType.StoredProcedure);
        }

        public void RemoveDrawOffer(string gameId)
        {
            using var connection = new MySqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("gameIdInput", gameId);

            connection.Execute("RemoveDrawOffer", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
