using Chess.Api.MoveValidation.Interfaces;

namespace Chess.Api.MoveValidation
{
    public class MoveHandler : IMoveHandler
    {
        public BoardState ApplyMove(BoardState currentState, Move move, MoveValidationResult moveValidation)
        {
            var newBoardState = new BoardState();

            Piece[,] piecePositions = (Piece[,]) currentState.PiecePositions.Clone();

            MovePiece(move, piecePositions);
            MovePiece(moveValidation.CastleRookMove, piecePositions);

            RemoveEnPassantCapture(moveValidation, piecePositions);
            PromotePawn(move, moveValidation, piecePositions);

            newBoardState.PiecePositions = piecePositions;
            newBoardState.EnPassantTarget = moveValidation.NewEnPassantTarget;
            newBoardState.ActiveColor = currentState.ActiveColor == Color.White ? Color.Black : Color.White;
            newBoardState.CastlingState = GetNewCastlingState(move, currentState);

            if (currentState.ActiveColor == Color.Black)
            {
                newBoardState.FullmoveNumber += 1;
            }

            return newBoardState;
        }

        private void MovePiece(Move move, Piece[,] piecePositions)
        {
            if (move == null)
            {
                return;
            }

            var movedPiece = piecePositions[move.StartPosition.X, move.StartPosition.Y];

            piecePositions[move.StartPosition.X, move.StartPosition.Y] = null;
            piecePositions[move.EndPosition.X, move.EndPosition.Y] = movedPiece;
        }

        private void RemoveEnPassantCapture(MoveValidationResult moveValidation, Piece[,] piecePositions)
        {
            var enPassantCapture = moveValidation.EnPassantCapture;
            if (enPassantCapture == null)
            {
                return;
            }

            piecePositions[enPassantCapture.X, enPassantCapture.Y] = null;
        }

        private void PromotePawn(Move move, MoveValidationResult moveValidation, Piece[,] piecePositions)
        {
            if (moveValidation.IsPromotion)
            {
                var piece = piecePositions[move.EndPosition.X, move.EndPosition.Y];
                piecePositions[move.EndPosition.X, move.EndPosition.Y] = new Piece
                {
                    Color = piece.Color,
                    Type = PieceType.Queen
                };
            }
        }

        private CastlingState GetNewCastlingState(Move move, BoardState currentBoardState)
        {
            var movedPiece = currentBoardState.PiecePositions[move.StartPosition.X, move.StartPosition.Y];

            if (movedPiece.Type == PieceType.King)
            {
                return GetCastlingStateForKingMovement(movedPiece, currentBoardState.CastlingState);
            }

            if (movedPiece.Type == PieceType.Rook)
            {
                return GetCastlingStateForRookMovement(movedPiece, move, currentBoardState.CastlingState);
            }

            return currentBoardState.CastlingState;
        }

        private CastlingState GetCastlingStateForKingMovement(Piece piece, CastlingState currentState) {
            if (piece.Color == Color.White)
            {
                return new CastlingState
                {
                    WhiteKingside = false,
                    WhiteQueenside = false,
                    BlackKingside = currentState.BlackKingside,
                    BlackQueenside = currentState.BlackQueenside
                };
            }

            return new CastlingState
            {
                WhiteKingside = currentState.WhiteKingside,
                WhiteQueenside = currentState.WhiteQueenside,
                BlackKingside = false,
                BlackQueenside = false
            };
        }

        private CastlingState GetCastlingStateForRookMovement(Piece piece, Move move, CastlingState currentState)
        {
            var newCastlingState = new CastlingState
            {
                WhiteKingside = currentState.WhiteKingside,
                WhiteQueenside = currentState.WhiteQueenside,
                BlackKingside = currentState.BlackKingside,
                BlackQueenside = currentState.BlackQueenside
            };

            if (piece.Color == Color.White) {
                if (move.StartPosition.X == 0 && move.StartPosition.Y == 0) {

                    newCastlingState.WhiteQueenside = false;

                } else if (move.StartPosition.X == 7 && move.StartPosition.Y == 0) {

                    newCastlingState.WhiteKingside = false;
                }

                return newCastlingState;
            }

            if (move.StartPosition.X == 0 && move.StartPosition.Y == 7) {

                newCastlingState.BlackQueenside = false;

            } else if (move.StartPosition.X == 7 && move.StartPosition.Y == 7) {

                newCastlingState.BlackKingside = false;
            }

            return newCastlingState;
        }
    }
}
