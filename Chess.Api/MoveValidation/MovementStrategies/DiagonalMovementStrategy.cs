using System;

namespace Chess.Api.MoveValidation.MovementStrategies
{
    public class DiagonalMovementStrategy : AbstractMovementStrategy
    {
        public override MoveValidationResult ValidateMove(Move move, BoardState boardState)
        {
            if (!IsSquareUsable(move.StartPosition, move.EndPosition, boardState.PiecePositions))
            {
                return new MoveValidationResult
                {
                    IsValid = false
                };
            }

            if (!IsDiagonal(move))
            {
                return new MoveValidationResult
                {
                    IsValid = false
                };
            }

            return new MoveValidationResult
            {
                IsValid = !IsBlocked(move, boardState.PiecePositions),
                ShouldResetHalfmoveClock = IsSquareOccupied(move.EndPosition, boardState.PiecePositions)
            };
        }

        private bool IsDiagonal(Move move)
        {
            return Math.Abs(move.EndPosition.X - move.StartPosition.X) ==
                   Math.Abs(move.EndPosition.Y - move.StartPosition.Y);
        }

        private bool IsBlocked(Move move, Piece[,] piecePositions)
        {
            var xIncrement = Math.Sign(move.EndPosition.X - move.StartPosition.X);
            var yIncrement = Math.Sign(move.EndPosition.Y - move.StartPosition.Y);

            var distance = Math.Abs(move.EndPosition.X - move.StartPosition.X);

            for (int step = 1; step < distance; step++)
            {
                var x = move.StartPosition.X + (xIncrement * step);
                var y = move.StartPosition.Y + (yIncrement * step);

                var pieceAtPosition = piecePositions[x, y];
                if (pieceAtPosition != null) return true;
            }

            return false;
        }
    }
}
