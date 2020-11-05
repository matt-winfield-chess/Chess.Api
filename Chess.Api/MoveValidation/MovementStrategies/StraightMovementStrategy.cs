using System;

namespace Chess.Api.MoveValidation.MovementStrategies
{
    public class StraightMovementStrategy : AbstractMovementStrategy
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

            if (!IsStraight(move))
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

        private bool IsStraight(Move move)
        {
            return move.StartPosition.X == move.EndPosition.X || move.StartPosition.Y == move.EndPosition.Y;
        }

        private bool IsBlocked(Move move, Piece[,] piecePositions)
        {
            var xIncrement = Math.Sign(move.EndPosition.X - move.StartPosition.X);
            var yIncrement = Math.Sign(move.EndPosition.Y - move.StartPosition.Y);

            var xDistance = Math.Abs(move.EndPosition.X - move.StartPosition.X);
            var yDistance = Math.Abs(move.EndPosition.Y - move.StartPosition.Y);

            var distance = Math.Max(xDistance, yDistance);

            for (int step = 1; step < distance; step++)
            {
                var x = move.StartPosition.X + (step * xIncrement);
                var y = move.StartPosition.Y + (step * yIncrement);

                var pieceAtPosition = piecePositions[x, y];
                if (pieceAtPosition != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
