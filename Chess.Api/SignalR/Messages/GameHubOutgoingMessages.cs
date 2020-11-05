namespace Chess.Api.SignalR.Messages
{
    public static class GameHubOutgoingMessages
    {
        public const string MOVE_PLAYED = "MovePlayed";
        public const string ILLEGAL_MOVE = "IllegalMove";
        public const string CHECKMATE = "Checkmate";
        public const string DRAW = "Draw";
        public const string MOVE_RECEIVED = "MoveReceived";
    }
}
