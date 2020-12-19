namespace Chess.Api.SignalR.Messages
{
    public static class GameHubOutgoingMessages
    {
        public const string MOVE_PLAYED = "MovePlayed";
        public const string ILLEGAL_MOVE = "IllegalMove";
        public const string CHECKMATE = "Checkmate";
        public const string DRAW = "Draw";
        public const string MOVE_RECEIVED = "MoveReceived";
        public const string RESIGNATION = "Resignation";
        public const string DRAW_OFFER = "DrawOffer";
        public const string DRAW_OFFER_ACCEPTED = "DrawOfferAccepted";
        public const string DRAW_OFFER_DECLINED = "DrawOfferDeclined";
    }
}
