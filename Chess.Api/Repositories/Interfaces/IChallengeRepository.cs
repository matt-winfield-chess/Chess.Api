﻿using Chess.Api.Models;
using System.Collections.Generic;

namespace Chess.Api.Repositories.Interfaces
{
    public interface IChallengeRepository
    {
        IEnumerable<ChallengeDatabaseModel> GetChallengesByChallenger(int challengerId);
        IEnumerable<ChallengeDatabaseModel> GetChallengesByRecipient(int recipientId);
        void CreateChallenge(int challengerId, int recipientId, ChallengerColor challengerColor);
        void DeleteChallenge(int challengerId, int recipientId);
    }
}
