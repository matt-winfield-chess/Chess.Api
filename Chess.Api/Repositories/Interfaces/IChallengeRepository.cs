using Chess.Api.Models;
using System.Collections.Generic;
using Chess.Api.Models.Database;

namespace Chess.Api.Repositories.Interfaces
{
    public interface IChallengeRepository : IHasHealthCheck
    {
        IEnumerable<ChallengeDatabaseModel> GetChallengesByChallenger(int challengerId);
        IEnumerable<ChallengeDatabaseModel> GetChallengesByRecipient(int recipientId);
        ChallengeDatabaseModel GetChallenge(int challengerId, int recipientId);
        void CreateChallenge(int challengerId, int recipientId, ChallengerColor challengerColor);
        void DeleteChallenge(int challengerId, int recipientId);
    }
}
