namespace Chess.Api.Constants
{
    public static class GameConstants
    {
        public const string STANDARD_START_POSITION_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public const string WHITE = "white";
        public const string BLACK = "black";

        public const string CHECKMATE_TERMINATION = "checkmate";
        public const string STALEMATE_TERMINATION = "stalemate";
        public const string FIFTY_MOVE_RULE_TERMINATION = "50-move-rule";
        public const string THREEFOLD_REPETITION_TERMINATION = "threefold-repetition";
        public const string RESIGNATION_TERMINATION = "resignation";
        public const string AGREEMENT_TERMINATION = "agreement";
    }
}
