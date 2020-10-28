namespace Chess.Api.MoveValidation
{
    public class BoardState
    {
        public Color ActiveColor { get; set; }
        public Coordinate EnPassantTarget { get; set; }
        public CastlingState CastlingState { get; set; }
        public int HalfmoveClock { get; set; }
        public int FullmoveNumber { get; set; }
        public Piece[,] PiecePositions { get; set; } = new Piece[8,8];

        public BoardState() { }

        public BoardState(Piece[,] piecePositions)
        {
            PiecePositions = piecePositions;
        }

        public Piece GetPiece(int x, int y)
        {
            return PiecePositions[x, y];
        }
    }
}
