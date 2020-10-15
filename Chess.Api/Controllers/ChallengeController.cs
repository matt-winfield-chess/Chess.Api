using Chess.Api.Models;
using Chess.Api.Repositories.Interfaces;
using Chess.Api.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChallengeController : Controller
    {
        private IChallengeRepository _challengeRepository;
        private IUserRepository _userRepository;

        public ChallengeController(IChallengeRepository challengeRepository, IUserRepository userRepository)
        {
            _challengeRepository = challengeRepository;
            _userRepository = userRepository;
        }

        [HttpGet("/receivedChallenges")]
        public ActionResult<ApiMethodResponse<IEnumerable<Challenge>>> GetReceivedChallenges()
        {
            var claims = HttpContext.User.Claims;
            var id = int.Parse(claims.FirstOrDefault(claim => claim.Type == "id")?.Value);

            var databaseChallenges = _challengeRepository.GetChallengesByRecipient(id);

            var challenges = MapDatabaseChallengeToDisplayChallenge(databaseChallenges);

            return Ok(new ApiMethodResponse<IEnumerable<Challenge>>() {
                Data = challenges
            });
        }

        [HttpGet("/sentChallenges")]
        public ActionResult<ApiMethodResponse<IEnumerable<Challenge>>> GetSentChallenges()
        {
            var claims = HttpContext.User.Claims;
            var id = int.Parse(claims.FirstOrDefault(claim => claim.Type == "id")?.Value);

            var databaseChallenges = _challengeRepository.GetChallengesByChallenger(id);

            var challenges = MapDatabaseChallengeToDisplayChallenge(databaseChallenges);

            return Ok(new ApiMethodResponse<IEnumerable<Challenge>>()
            {
                Data = challenges
            });
        }

        [HttpPost("/sendChallenge")]
        public ActionResult<ApiMethodResponse<bool>> PostChallenge([FromBody] PostChallengeModel challengeModel)
        {
            var claims = HttpContext.User.Claims;
            var id = int.Parse(claims.FirstOrDefault(claim => claim.Type == "id")?.Value);

            var recipient = _userRepository.GetUserCredentialsByUsername(challengeModel.Username);

            if (recipient == null)
            {
                return NotFound(new ApiMethodResponse<bool>
                {
                    Errors = new string[] { $"User '{challengeModel.Username}' could not be found!" }
                });
            }

            _challengeRepository.CreateChallenge(id, recipient.UserId, challengeModel.ChallengerColor);

            return Ok(new ApiMethodResponse<bool>());
        }

        [HttpDelete("{challengerId}/{recipientId}")]
        public ActionResult<ApiMethodResponse<bool>> DeleteChallenge(int challengerId, int recipientId)
        {
            var claims = HttpContext.User.Claims;
            var id = int.Parse(claims.FirstOrDefault(claim => claim.Type == "id")?.Value);

            _challengeRepository.DeleteChallenge(challengerId, recipientId);

            return Ok(new ApiMethodResponse<bool>());
        }

        private IEnumerable<Challenge> MapDatabaseChallengeToDisplayChallenge(IEnumerable<ChallengeDatabaseModel> databaseChallenges)
        {
            return databaseChallenges.Select(challenge =>
            {
                var challenger = _userRepository.GetUserById(challenge.ChallengerId);
                var recipient = _userRepository.GetUserById(challenge.RecipientId);
                return new Challenge
                {
                    Challenger = challenger,
                    Recipient = recipient,
                    ChallengerColor = challenge.ChallengerColor
                };
            });
        }
    }
}
