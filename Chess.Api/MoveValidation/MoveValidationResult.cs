namespace Chess.Api.MoveValidation
{
    public class MoveValidationResult
    {
        public bool IsValid { get; set; }
        public Move CastleRookMove { get; set; }
        public Coordinate NewEnPassantTarget { get; set; }
        public Coordinate EnPassantCapture { get; set; }
        public PieceType? Promotion { get; set; }
        public bool ShouldResetHalfmoveClock { get; set; }
    }
}
