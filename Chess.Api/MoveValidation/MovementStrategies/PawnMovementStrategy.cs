using System;

namespace Chess.Api.MoveValidation.MovementStrategies
{
    public class PawnMovementStrategy : AbstractMovementStrategy
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

            if (!IsMovingInCorrectDirection(move, boardState.PiecePositions))
            {
                return new MoveValidationResult
                {
                    IsValid = false
                };
            }

            if (IsEnPassantCapture(move, boardState))
            {
                return new MoveValidationResult()
                {
                    IsValid = true,
                    EnPassantCapture = GetEnPassantCapturedCoordinate(move)
                };
            }

            if (IsCapture(move, boardState.PiecePositions))
            {
                return new MoveValidationResult
                {
                    IsValid = true,
                    IsPromotion = IsPromotion(move)
                };
            }

            return new MoveValidationResult
            {
                IsValid = IsValidMoveForward(move, boardState.PiecePositions),
                NewEnPassantTarget = GetNewEnPassantTarget(move),
                IsPromotion = IsPromotion(move)
            };
        }

        private bool IsMovingInCorrectDirection(Move move, Piece[,] piecePositions)
        {
            var piece = piecePositions[move.StartPosition.X, move.StartPosition.Y];
            var correctDirection = GetPieceCorrectDirection(piece);
            return Math.Sign(move.EndPosition.Y - move.StartPosition.Y) == correctDirection;
        }

        private bool IsEnPassantCapture(Move move, BoardState boardState)
        {
            if (boardState.EnPassantTarget == null)
            {
                return false;
            }

            if (boardState.EnPassantTarget.X != move.StartPosition.X - 1 &&
                boardState.EnPassantTarget.X != move.StartPosition.X + 1)
            {
                return false;
            }

            if (Math.Abs(move.EndPosition.Y - move.StartPosition.Y) != 1)
            {
                return false;
            }

            return boardState.EnPassantTarget.X == move.EndPosition.X &&
                   boardState.EnPassantTarget.Y == move.EndPosition.Y;
        }

        private bool IsCapture(Move move, Piece[,] piecePositions)
        {
            if (Math.Abs(move.EndPosition.X - move.StartPosition.X) != 1 ||
                Math.Abs(move.EndPosition.Y - move.StartPosition.Y) != 1)
            {
                return false;
            }

            var movingPiece = piecePositions[move.StartPosition.X, move.StartPosition.Y];
            var capturedPiece = piecePositions[move.EndPosition.X, move.EndPosition.Y];

            if (capturedPiece == null)
            {
                return false;
            }

            return movingPiece.Color != capturedPiece.Color;
        }

        private bool IsValidMoveForward(Move move, Piece[,] piecePositions)
        {
            if (move.StartPosition.X != move.EndPosition.X)
            {
                return false;
            }

            var piece = piecePositions[move.StartPosition.X, move.StartPosition.Y];

            var correctDirection = GetPieceCorrectDirection(piece);
            var maxDistance = CanMoveTwoSquares(piece, move.StartPosition.Y) ? 2 : 1;

            var distance = move.EndPosition.Y - move.StartPosition.Y;
            if (Math.Sign(distance) != correctDirection)
            {
                return false;
            }

            if (IsForwardMovementBlocked(move, distance, correctDirection, piecePositions))
            {
                return false;
            }

            return Math.Abs(distance) > 0 && Math.Abs(distance) <= maxDistance;
        }

        private bool IsForwardMovementBlocked(Move move, int distance, int correctDirection, Piece[,] piecePositions)
        {
            for (int i = 1; i <= Math.Abs(distance); i++)
            {
                var piece = piecePositions[move.StartPosition.X, move.StartPosition.Y + (i * correctDirection)];
                if (piece != null)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsPromotion(Move move)
        {
            return move.EndPosition.Y == 0 || move.EndPosition.Y == 7;
        }

        private Coordinate GetEnPassantCapturedCoordinate(Move move)
        {
            return new Coordinate
            {
                X = move.EndPosition.X,
                Y = move.StartPosition.Y
            };
        }

        private Coordinate GetNewEnPassantTarget(Move move)
        {
            if (Math.Abs(move.EndPosition.Y - move.StartPosition.Y) != 2)
            {
                return null;
            }

            var direction = Math.Sign(move.EndPosition.Y - move.StartPosition.Y);
            return new Coordinate
            {
                X = move.StartPosition.X,
                Y = move.StartPosition.Y + direction
            };
        }

        private int GetPieceCorrectDirection(Piece piece)
        {
            return piece.Color == Color.White ? 1 : -1;
        }

        private bool CanMoveTwoSquares(Piece piece, int yPosition)
        {
            if (piece.Color == Color.White)
            {
                return yPosition == 1;
            }

            return yPosition == 6;
        }
    }
}
