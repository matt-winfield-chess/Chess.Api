namespace Chess.Api.Models
{
    public class Challenge
    {
        public User Challenger { get; set; }
        public User Recipient { get; set; }
        public ChallengerColor ChallengerColor { get; set; }
    }
}
