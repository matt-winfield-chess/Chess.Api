namespace Chess.Api.MoveValidation.MovementStrategies
{
    public abstract class AbstractMovementStrategy : IMovementStrategy
    {
        public abstract MoveValidationResult ValidateMove(Move move, BoardState boardState);

        protected bool IsSquareUsable(Coordinate startPosition, Coordinate endPosition, Piece[,] piecePositions)
        {
            var movingPiece = piecePositions[startPosition.X, startPosition.Y];
            var targetSquarePiece = piecePositions[endPosition.X, endPosition.Y];

            return targetSquarePiece == null || movingPiece.Color != targetSquarePiece.Color;
        }
    }
}
