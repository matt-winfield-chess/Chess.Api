using Chess.Api.Models;
using System.Collections.Generic;
using Chess.Api.Models.Database;

namespace Chess.Api.Repositories.Interfaces
{
    public interface IGameRepository
    {
        void CreateGame(string gameId, int whitePlayerId, int blackPlayerId);
        void CreateGame(string gameId, int whitePlayerId, int blackPlayerId, string fen);
        GameDatabaseModel GetGameById(string gameId);
        void AddMoveToGame(string gameId, string move, string newFen);
        IEnumerable<MoveDatabaseModel> GetMovesByGameId(string gameId);
        IEnumerable<GameDatabaseModel> GetUserActiveGames(int userId);
    }
}
