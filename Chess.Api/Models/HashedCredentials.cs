namespace Chess.Api.Models
{
    public class HashedCredentials
    {
        public int UserId { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
    }
}
