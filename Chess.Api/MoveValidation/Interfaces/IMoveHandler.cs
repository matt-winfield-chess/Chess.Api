namespace Chess.Api.MoveValidation.Interfaces
{
    public interface IMoveHandler
    {
        BoardState ApplyMove(BoardState currentState, Move move, MoveValidationResult moveValidation);
    }
}
