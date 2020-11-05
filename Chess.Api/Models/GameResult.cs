using Chess.Api.MoveValidation;

namespace Chess.Api.Models
{
    public class GameResult
    {
        public Color? WinnerColor { get; set; }
        public string Termination { get; set; }
    }
}
