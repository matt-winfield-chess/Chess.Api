using Chess.Api.MoveValidation.Interfaces;

namespace Chess.Api.MoveValidation
{
    public class MoveValidator : IMoveValidator
    {
        private FenParser _fenParser = new FenParser();
        private CoordinateNotationParser _coordinateNotationParser = new CoordinateNotationParser();
        private MovementStrategyProvider _movementStrategyProvider = new MovementStrategyProvider();

        public MoveValidationResult ValidateMove(string currentFen, string move)
        {
            var boardState = _fenParser.ParseFen(currentFen);

            var parsedMove = _coordinateNotationParser.ParseNotationMove(move);
            var piece = boardState.PiecePositions[parsedMove.StartPosition.X, parsedMove.StartPosition.Y];

            if (!IsCorrectColor(piece, boardState) || parsedMove.StartPosition.Equals(parsedMove.EndPosition))
            {
                return new MoveValidationResult
                {
                    IsValid = false
                };
            }

            var movementStrategies = _movementStrategyProvider.GetStrategies(piece.Type);

            foreach (var movementStrategy in movementStrategies)
            {
                var movementValidationResult = movementStrategy.ValidateMove(parsedMove, boardState);

                if (movementValidationResult.IsValid)
                {
                    return movementValidationResult;
                }
            }

            return new MoveValidationResult {IsValid = false};
        }

        private bool IsCorrectColor(Piece piece, BoardState boardState)
        {
            return piece.Color == boardState.ActiveColor;
        }
    }
}
