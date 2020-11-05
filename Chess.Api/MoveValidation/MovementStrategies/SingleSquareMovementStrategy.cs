using System;

namespace Chess.Api.MoveValidation.MovementStrategies
{
    public class SingleSquareMovementStrategy : AbstractMovementStrategy
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

            return new MoveValidationResult
            {
                IsValid = IsSquareReachable(move),
                ShouldResetHalfmoveClock = IsSquareOccupied(move.EndPosition, boardState.PiecePositions)
            };
        }

        private bool IsSquareReachable(Move move)
        {
            return Math.Abs(move.EndPosition.X - move.StartPosition.X) <= 1
                   && Math.Abs(move.EndPosition.Y - move.StartPosition.Y) <= 1;
        }
    }
}
