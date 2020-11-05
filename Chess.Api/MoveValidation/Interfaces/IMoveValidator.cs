using System.Collections.Generic;
using Chess.Api.Models.Database;

namespace Chess.Api.MoveValidation.Interfaces
{
    public interface IMoveValidator
    {
        MoveValidationResult ValidateMove(string currentFen, string move);
        MoveValidationResult ValidateMove(BoardState boardState, Move move);
        bool IsCheckmate(BoardState boardState);
        bool IsStalemate(BoardState boardState);
        bool IsThreefoldRepetition(BoardState boardState, IEnumerable<PositionDatabaseModel> previousStates);
    }
}
