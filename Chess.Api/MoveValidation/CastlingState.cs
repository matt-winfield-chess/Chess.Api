namespace Chess.Api.MoveValidation
{
    public class CastlingState
    {
        public bool WhiteKingside { get; set; }
        public bool WhiteQueenside { get; set; }
        public bool BlackKingside { get; set; }
        public bool BlackQueenside { get; set; }
    }
}
