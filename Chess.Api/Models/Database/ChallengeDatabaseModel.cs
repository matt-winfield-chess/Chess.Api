namespace Chess.Api.Models.Database
{
    public class ChallengeDatabaseModel
    {
        public int ChallengerId { get; set; }
        public int RecipientId { get; set; }
        public ChallengerColor ChallengerColor { get; set; }
        public bool Active { get; set; }
    }
}
