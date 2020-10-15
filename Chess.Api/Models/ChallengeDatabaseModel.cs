using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.Api.Models
{
    public class ChallengeDatabaseModel
    {
        public int ChallengerId { get; set; }
        public int RecipientId { get; set; }
        public ChallengerColor ChallengerColor { get; set; }
    }
}
