namespace Chess.Api.MoveValidation.MovementStrategies
{
    public interface IMovementStrategy
    {
        MoveValidationResult ValidateMove(Move move, BoardState boardState);
    }
}
