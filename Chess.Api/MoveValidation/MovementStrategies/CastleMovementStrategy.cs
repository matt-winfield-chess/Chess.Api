using System;

namespace Chess.Api.MoveValidation.MovementStrategies
{
    public class CastleMovementStrategy : AbstractMovementStrategy
    {
        public override MoveValidationResult ValidateMove(Move move, BoardState boardState)
        {
            if (!IsCastlingAvailable(move, boardState))
            {
                return new MoveValidationResult
                {
                    IsValid = false
                };
            }

            if (IsBlocked(move, boardState.PiecePositions))
            {
                return new MoveValidationResult
                {
                    IsValid = false
                };
            }

            if (IsThroughCheck(move, boardState))
            {
                return new MoveValidationResult
                {
                    IsValid = false
                };
            }

            var rookMove = GetRookMove(move);
            return new MoveValidationResult
            {
                IsValid = true,
                CastleRookMove = rookMove
            };
        }

        private bool IsCastlingAvailable(Move move, BoardState boardState)
        {
            var castlingState = boardState.CastlingState;

            var piece = boardState.PiecePositions[move.StartPosition.X, move.StartPosition.Y];

            if (piece.Type != PieceType.King)
            {
                return false;
            }

            if (move.EndPosition.X != 2 && move.EndPosition.X != 6)
            {
                return false;
            }

            if (piece.Color == Color.White)
            {
                if (move.EndPosition.Y != 0) return false;

                if (move.EndPosition.X == 2 && castlingState.WhiteQueenside == false) return false;
                if (move.EndPosition.X == 6 && castlingState.WhiteKingside == false) return false;
                return true;
            }

            if (move.EndPosition.Y != 7) return false;

            if (move.EndPosition.X == 2 && castlingState.BlackQueenside == false) return false;
            if (move.EndPosition.X == 6 && castlingState.BlackKingside == false) return false;
            return true;
        }

        private bool IsBlocked(Move move, Piece[,] piecePositions)
        {
            if (move.EndPosition.X == 2)
            {
                for (int x = 1; x <= 3; x++)
                {
                    var piece = piecePositions[x, move.EndPosition.Y];
                    if (piece != null) return true;
                }
            }
            else
            {
                for (int x = 5; x <= 6; x++)
                {
                    var piece = piecePositions[x, move.EndPosition.Y];
                    if (piece != null) return true;
                }
            }

            return false;
        }

        private bool IsThroughCheck(Move move, BoardState boardState)
        {
            var color = boardState.PiecePositions[move.StartPosition.X, move.StartPosition.Y].Color;

            if (MoveValidator.IsSquareAttacked(move.StartPosition, color, boardState))
            {
                return true;
            }

            for (int x = move.StartPosition.X;
                x != move.EndPosition.X;
                x += Math.Sign(move.EndPosition.X - move.StartPosition.X))
            {
                var position = new Coordinate
                {
                    X = x,
                    Y = move.StartPosition.Y
                };

                if (MoveValidator.IsSquareAttacked(position, color, boardState))
                {
                    return true;
                }
            }

            return false;
        }

        private Move GetRookMove(Move move)
        {
            if (move.EndPosition.X == 2) // Queenside castle
            {
                return new Move
                {
                    StartPosition = new Coordinate
                    {
                        X = 0,
                        Y = move.StartPosition.Y
                    },
                    EndPosition = new Coordinate
                    {
                        X = 3,
                        Y = move.StartPosition.Y
                    }
                };
            }

            return new Move
            {
                StartPosition = new Coordinate
                {
                    X = 7,
                    Y = move.StartPosition.Y
                },
                EndPosition = new Coordinate
                {
                    X = 5,
                    Y = move.StartPosition.Y
                }
            };
        }
    }
}
