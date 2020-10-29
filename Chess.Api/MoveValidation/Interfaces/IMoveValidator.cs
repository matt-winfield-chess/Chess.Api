namespace Chess.Api.MoveValidation.Interfaces
{
    public interface IMoveValidator
    {
        MoveValidationResult ValidateMove(string currentFen, string move);
    }
}
