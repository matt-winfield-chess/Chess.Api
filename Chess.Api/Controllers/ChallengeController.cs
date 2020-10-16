using Chess.Api.Models;
using Chess.Api.Repositories.Interfaces;
using Chess.Api.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Chess.Api.SignalR.Hubs;
using System.Threading.Tasks;
using Chess.Api.SignalR.Messages;

namespace Chess.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChallengeController : Controller
    {
        private IChallengeRepository _challengeRepository;
        private IUserRepository _userRepository;
        private IHubContext<ChallengeHub> _challengeHubContext;

        public ChallengeController(IChallengeRepository challengeRepository, IUserRepository userRepository, IHubContext<ChallengeHub> challengeHubContext)
        {
            _challengeRepository = challengeRepository;
            _userRepository = userRepository;
            _challengeHubContext = challengeHubContext;
        }

        [HttpGet("receivedChallenges")]
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

        [HttpGet("sentChallenges")]
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

        [HttpPost("sendChallenge")]
        public async Task<ActionResult<ApiMethodResponse<bool>>> PostChallenge([FromBody] PostChallengeModel challengeModel)
        {
            var claims = HttpContext.User.Claims;
            var id = int.Parse(claims.FirstOrDefault(claim => claim.Type == "id")?.Value);

            var challenger = _userRepository.GetUserById(id);
            var recipient = _userRepository.GetUserCredentialsByUsername(challengeModel.Username);

            if (recipient == null)
            {
                return NotFound(new ApiMethodResponse<bool>
                {
                    Errors = new string[] { $"User '{challengeModel.Username}' could not be found!" }
                });
            }

            try
            {
                _challengeRepository.CreateChallenge(id, recipient.UserId, challengeModel.ChallengerColor);

                await SendNewChallengeMessage(challengeModel, challenger, recipient);

                return Ok(new ApiMethodResponse<bool>());
            } catch
            {
                return BadRequest(new ApiMethodResponse<bool>
                {
                    Errors = new string[] { $"Challenge has already been sent to '{challengeModel.Username}'" }
                });
            }
        }

        [HttpDelete("delete/{challengerId}/{recipientId}")]
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

        private async Task SendNewChallengeMessage(PostChallengeModel challengeModel, User challenger, HashedCredentials recipient)
        {
            var user = _challengeHubContext.Clients.User(recipient.UserId.ToString());
            await user.SendAsync(ChallengeHubOutgoingMessages.NEW_CHALLENGE, new Challenge
            {
                Challenger = challenger,
                Recipient = new User
                {
                    Id = recipient.UserId,
                    Username = challengeModel.Username
                },
                ChallengerColor = challengeModel.ChallengerColor
            });
        }
    }
}
