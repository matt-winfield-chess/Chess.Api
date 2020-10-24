using Chess.Api.Models;
using System.Collections.Generic;
using Chess.Api.Models.Database;

namespace Chess.Api.Repositories.Interfaces
{
    public interface IGameRepository
    {
        void CreateGame(string gameId, int whitePlayerId, int blackPlayerId);
        GameDatabaseModel GetGameById(string gameId);
        void AddMoveToGame(string gameId, string move);
        IEnumerable<MoveDatabaseModel> GetMovesByGameId(string gameId);
        IEnumerable<GameDatabaseModel> GetUserActiveGames(int userId);
    }
}
