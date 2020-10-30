using System.Collections.Generic;
using System.Linq;
using Chess.Api.MoveValidation.Interfaces;

namespace Chess.Api.MoveValidation
{
    public class MoveValidator : IMoveValidator
    {
        private readonly FenParser _fenParser = new FenParser();
        private readonly CoordinateNotationParser _coordinateNotationParser = new CoordinateNotationParser();
        private readonly MovementStrategyProvider _movementStrategyProvider = new MovementStrategyProvider();
        private readonly MoveHandler _moveHandler = new MoveHandler();

        public MoveValidationResult ValidateMove(string currentFen, string move)
        {
            var boardState = _fenParser.ParseFen(currentFen);

            var parsedMove = _coordinateNotationParser.ParseNotationMove(move);
            var piece = boardState.PiecePositions[parsedMove.StartPosition.X, parsedMove.StartPosition.Y];

            if (piece == null)
            {
                return new MoveValidationResult
                {
                    IsValid = false
                };
            }

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
                    var newBoardState = _moveHandler.ApplyMove(boardState, parsedMove, movementValidationResult);

                    if (IsKingInCheck(boardState.ActiveColor, newBoardState))
                    {
                        return new MoveValidationResult {IsValid = false};
                    }

                    return movementValidationResult;
                }
            }

            return new MoveValidationResult {IsValid = false};
        }

        private bool IsCorrectColor(Piece piece, BoardState boardState)
        {
            return piece.Color == boardState.ActiveColor;
        }

        private bool IsKingInCheck(Color kingColor, BoardState boardState)
        {
            var kingPosition = FindKing(kingColor, boardState);

            if (kingPosition == null)
            {
                return false;
            }

            return IsKingAttackedInStraightLine(kingPosition, kingColor, boardState)
                   || IsKingAttackedDiagonally(kingPosition, kingColor, boardState)
                   || IsKingAttackedByPawn(kingPosition, kingColor, boardState)
                   || IsKingAttackedByKnight(kingPosition, kingColor, boardState)
                   || IsKingAttackedByKing(kingPosition, kingColor, boardState);
        }

        private Coordinate FindKing(Color kingColor, BoardState boardState)
        {
            for (int x = 0; x < boardState.PiecePositions.GetLength(0); x++)
            {
                for (int y = 0; y < boardState.PiecePositions.GetLength(1); y++)
                {
                    var piece = boardState.PiecePositions[x, y];
                    if (piece != null && piece.Type == PieceType.King && piece.Color == kingColor)
                    {
                        return new Coordinate
                        {
                            X = x,
                            Y = y
                        };
                    }
                }
            }

            return null;
        }

        private bool IsKingAttackedInStraightLine(Coordinate kingPosition, Color kingColor, BoardState boardState)
        {
            var validPieces = new[] {PieceType.Queen, PieceType.Rook};


            return IsKingAttackedByLineOfSightPiece(kingPosition, kingColor, boardState, 1, 0, validPieces)
                   || IsKingAttackedByLineOfSightPiece(kingPosition, kingColor, boardState, -1, 0, validPieces)
                   || IsKingAttackedByLineOfSightPiece(kingPosition, kingColor, boardState, 0, 1, validPieces)
                   || IsKingAttackedByLineOfSightPiece(kingPosition, kingColor, boardState, 0, -1, validPieces);
        }

        private bool IsKingAttackedDiagonally(Coordinate kingPosition, Color kingColor, BoardState boardState)
        {
            var validPieces = new[] {PieceType.Bishop, PieceType.Queen};

            return IsKingAttackedByLineOfSightPiece(kingPosition, kingColor, boardState, 1, 1, validPieces)
                   || IsKingAttackedByLineOfSightPiece(kingPosition, kingColor, boardState, 1, -1, validPieces)
                   || IsKingAttackedByLineOfSightPiece(kingPosition, kingColor, boardState, -1, 1, validPieces)
                   || IsKingAttackedByLineOfSightPiece(kingPosition, kingColor, boardState, -1, -1, validPieces);
        }

        private bool IsKingAttackedByPawn(Coordinate kingPosition, Color kingColor, BoardState boardState)
        {
            var opponentMovementDirection = kingColor == Color.White ? -1 : 1;


            if (kingPosition.X - 1 >= 0)
            {
                var candidateAttacker1 = boardState.PiecePositions[kingPosition.X - 1, kingPosition.Y - opponentMovementDirection];
                if (candidateAttacker1 != null && candidateAttacker1.Type == PieceType.Pawn && candidateAttacker1.Color != kingColor)
                {
                    return true;
                }
            }

            if (kingPosition.X + 1 < boardState.PiecePositions.GetLength(0))
            {
                var candidateAttacker2 = boardState.PiecePositions[kingPosition.X + 1, kingPosition.Y - opponentMovementDirection];
                if (candidateAttacker2 != null && candidateAttacker2.Type == PieceType.Pawn && candidateAttacker2.Color != kingColor)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsKingAttackedByKnight(Coordinate kingPosition, Color kingColor, BoardState boardState)
        {
            return DoesSquareHaveOpponentKnight(kingPosition.X + 2, kingPosition.Y + 1, kingColor, boardState)
                   || DoesSquareHaveOpponentKnight(kingPosition.X + 2, kingPosition.Y - 1, kingColor, boardState)
                   || DoesSquareHaveOpponentKnight(kingPosition.X - 2, kingPosition.Y + 1, kingColor, boardState)
                   || DoesSquareHaveOpponentKnight(kingPosition.X - 2, kingPosition.Y - 1, kingColor, boardState)
                   || DoesSquareHaveOpponentKnight(kingPosition.X + 1, kingPosition.Y + 2, kingColor, boardState)
                   || DoesSquareHaveOpponentKnight(kingPosition.X + 1, kingPosition.Y - 2, kingColor, boardState)
                   || DoesSquareHaveOpponentKnight(kingPosition.X - 1, kingPosition.Y + 2, kingColor, boardState)
                   || DoesSquareHaveOpponentKnight(kingPosition.X - 1, kingPosition.Y - 2, kingColor, boardState);
        }

        private bool IsKingAttackedByKing(Coordinate kingPosition, Color kingColor, BoardState boardState)
        {
            for (int x = kingPosition.X - 1; x <= kingPosition.X + 1; x++)
            {
                for (int y = kingPosition.Y - 1; y <= kingPosition.Y + 1; y++)
                {
                    if (x < 0 || x >= boardState.PiecePositions.GetLength(0)
                              || y < 0 || y >= boardState.PiecePositions.GetLength(1))
                    {
                        continue;
                    }

                    if (x == kingPosition.X && y == kingPosition.Y)
                    {
                        continue;
                    }

                    var piece = boardState.PiecePositions[x, y];
                    if (piece != null)
                    {
                        if (piece.Type == PieceType.King && piece.Color != kingColor)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool DoesSquareHaveOpponentKnight(int x, int y, Color kingColor, BoardState boardState) {
            if (x < 0 || x >= boardState.PiecePositions.GetLength(0) || y < 0 ||
                y >= boardState.PiecePositions.GetLength(1))
            {
                return false;
            }

            var piece = boardState.PiecePositions[x, y];
            if (piece == null)
            {
                return false;
            }

            return piece.Type == PieceType.Knight && piece.Color != kingColor;
        }

    private bool IsKingAttackedByLineOfSightPiece(Coordinate kingPosition, Color kingColor, BoardState boardState, 
            int xIncrement, int yIncrement, IEnumerable<PieceType> validPieces) {

            var x = kingPosition.X + xIncrement;
            var y = kingPosition.Y + yIncrement;

            while (x >= 0 && x < boardState.PiecePositions.GetLength(0) && y >= 0 && y < boardState.PiecePositions.GetLength(1))
            {
                var piece = boardState.PiecePositions[x, y];
                if (piece != null) {
                    if (piece.Color != kingColor) {
                        if (validPieces.Any(type => piece.Type == type)) {
                            return true;
                        }
                        break;
                    } else {
                        break;
                    }
                }
                x += xIncrement;
                y += yIncrement;
            }
            return false;
        }
    }
}
