using Chess.Api.MoveValidation;

namespace Chess.Api.Models.Database
{
    public class GameDatabaseModel
    {
        public string Id { get; set; }
        public int WhitePlayerId { get; set; }
        public int BlackPlayerId { get; set; }
        public bool Active { get; set; }
        public string Fen { get; set; }
        public string Winner { get; set; }
        public int? WinnerId { get; set; }
        public string Termination { get; set; }

        public int GetPlayerId(Color color)
        {
            return color == Color.White ? WhitePlayerId : BlackPlayerId;
        }
    }
}
