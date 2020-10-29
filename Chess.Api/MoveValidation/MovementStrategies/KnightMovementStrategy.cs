using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.Api.MoveValidation.MovementStrategies
{
    public class KnightMovementStrategy : AbstractMovementStrategy
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
                IsValid = IsSquareReachable(move)
            };
        }

        private bool IsSquareReachable(Move move)
        {
            return (Math.Abs(move.EndPosition.X - move.StartPosition.X) == 2 &&
                    Math.Abs(move.EndPosition.Y - move.StartPosition.Y) == 1)
                   || (Math.Abs(move.EndPosition.Y - move.StartPosition.Y) == 2 &&
                       Math.Abs(move.EndPosition.X - move.StartPosition.X) == 1);
        }
    }
}
