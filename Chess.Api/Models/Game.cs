using System.Collections.Generic;

namespace Chess.Api.Models
{
    public class Game
    {
        public string Id { get; set; }
        public User WhitePlayer { get; set; }
        public User BlackPlayer { get; set; }
        public IEnumerable<MoveModel> Moves { get; set; }
        public bool Active { get; set; }
    }
}