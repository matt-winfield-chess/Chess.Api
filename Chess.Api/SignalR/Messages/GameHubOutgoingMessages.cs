namespace Chess.Api.SignalR.Messages
{
    public static class GameHubOutgoingMessages
    {
        public const string MOVE_PLAYED = "MovePlayed";
        public const string ILLEGAL_MOVE = "IllegalMove";
    }
}
