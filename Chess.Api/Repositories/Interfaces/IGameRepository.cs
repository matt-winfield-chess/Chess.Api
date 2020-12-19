using Chess.Api.Models;
using System.Collections.Generic;
using Chess.Api.Models.Database;
using Chess.Api.MoveValidation;

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
        void SetGameResult(string gameId, string winnerColor, int? winnerId, string termination);
        IEnumerable<PositionDatabaseModel> GetPositionsSinceIrreversibleMove(string gameId);
        void AddPositionToGame(string gameId, string fen);
        void ClearPositionsFromIrreversibleMove(string gameId);
        void CreateDrawOffer(string gameId, string color);
        void RemoveDrawOffer(string gameId);
    }
}
