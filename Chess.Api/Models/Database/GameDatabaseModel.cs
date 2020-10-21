namespace Chess.Api.Models.Database
{
    public class GameDatabaseModel
    {
        public string Id { get; set; }
        public int WhitePlayerId { get; set; }
        public int BlackPlayerId { get; set; }
        public bool Active { get; set; }
    }
}
