using Chess.Api.Models;
using Chess.Api.Models.Database;
using Chess.Api.Models.Post;
using Chess.Api.Repositories.Interfaces;
using Chess.Api.Responses;
using Chess.Api.SignalR.Hubs;
using Chess.Api.SignalR.Messages;
using Chess.Api.Utils.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChallengeController : Controller
    {
        private readonly IChallengeRepository _challengeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IStringIdGenerator _stringIdGenerator;
        private readonly IClaimsProvider _claimsProvider;
        private readonly IHubContext<ChallengeHub> _challengeHubContext;
        private readonly Random _random = new Random();

        public ChallengeController(IChallengeRepository challengeRepository, IUserRepository userRepository, IGameRepository gameRepository,
            IStringIdGenerator stringIdGenerator, IClaimsProvider claimsProvider, IHubContext<ChallengeHub> challengeHubContext)
        {
            _challengeRepository = challengeRepository;
            _userRepository = userRepository;
            _gameRepository = gameRepository;
            _stringIdGenerator = stringIdGenerator;
            _claimsProvider = claimsProvider;
            _challengeHubContext = challengeHubContext;
        }

        [HttpGet("receivedChallenges")]
        public ActionResult<ApiMethodResponse<IEnumerable<Challenge>>> GetReceivedChallenges()
        {
            var id = _claimsProvider.GetId(HttpContext);
            if (id == null)
            {
                return Unauthorized(new ApiMethodResponse<IEnumerable<Challenge>>
                {
                    Errors = new[] {"Invalid ID in token"}
                });
            }

            var databaseChallenges = _challengeRepository.GetChallengesByRecipient(id.Value);

            var challenges = MapDatabaseChallengeToDisplayChallenge(databaseChallenges);

            return Ok(new ApiMethodResponse<IEnumerable<Challenge>>
            {
                Data = challenges
            });
        }

        [HttpGet("sentChallenges")]
        public ActionResult<ApiMethodResponse<IEnumerable<Challenge>>> GetSentChallenges()
        {
            var id = _claimsProvider.GetId(HttpContext);
            if (id == null)
            {
                return Unauthorized(new ApiMethodResponse<IEnumerable<Challenge>>
                {
                    Errors = new[] { "Invalid ID in token" }
                });
            }

            var databaseChallenges = _challengeRepository.GetChallengesByChallenger(id.Value);

            var challenges = MapDatabaseChallengeToDisplayChallenge(databaseChallenges);

            return Ok(new ApiMethodResponse<IEnumerable<Challenge>>
            {
                Data = challenges
            });
        }

        [HttpPost("sendChallenge")]
        public async Task<ActionResult<ApiMethodResponse<object>>> PostChallenge([FromBody] PostChallengeModel challengeModel)
        {
            var id = _claimsProvider.GetId(HttpContext);

            if (id == null)
            {
                return Unauthorized(new ApiMethodResponse<IEnumerable<object>>
                {
                    Errors = new[] { "Invalid ID in token" }
                });
            }

            var challenger = _userRepository.GetUserById(id.Value);
            var recipient = _userRepository.GetUserCredentialsByUsername(challengeModel.Username);

            if (recipient == null)
            {
                return NotFound(new ApiMethodResponse<object>
                {
                    Errors = new [] { $"User '{challengeModel.Username}' could not be found!" }
                });
            }

            try
            {
                _challengeRepository.CreateChallenge(id.Value, recipient.UserId, challengeModel.ChallengerColor);

                await SendNewChallengeMessage(challengeModel, challenger, recipient);

                return Ok(new ApiMethodResponse<object>());
            } catch
            {
                return BadRequest(new ApiMethodResponse<object>
                {
                    Errors = new [] { $"Challenge has already been sent to '{challengeModel.Username}'" }
                });
            }
        }

        [HttpPost("acceptChallenge")]
        public async Task<ActionResult<ApiMethodResponse<Game>>> AcceptChallenge([FromBody] PostChallengeAcceptModel challengeAcceptModel) {
            var id = _claimsProvider.GetId(HttpContext);
            if (id == null || id != challengeAcceptModel.RecipientId)
            {
                return Unauthorized(new ApiMethodResponse<Game>
                {
                    Errors = new [] {"This challenge can not be accepted as you are not the recipient"}
                });
            }

            var challenge = _challengeRepository.GetChallenge(challengeAcceptModel.ChallengerId, challengeAcceptModel.RecipientId);

            if (challenge == null)
            {
                return NotFound(new ApiMethodResponse<Game>
                {
                    Errors = new[]
                    {
                        $"The specified challenge with challenger {challengeAcceptModel.ChallengerId} and recipient {challengeAcceptModel.RecipientId} was not found"
                    }
                });
            }

            var challenger = _userRepository.GetUserById(challengeAcceptModel.ChallengerId);
            var recipient = _userRepository.GetUserById(challengeAcceptModel.RecipientId);

            var game = CreateNewGame(challenge, challenger, recipient);

            _challengeRepository.DeleteChallenge(challengeAcceptModel.ChallengerId, challengeAcceptModel.RecipientId);

            await _challengeHubContext.Clients.User(challengeAcceptModel.ChallengerId.ToString())
                .SendAsync(ChallengeHubOutgoingMessages.CHALLENGE_ACCEPTED, game);

            return Ok(new ApiMethodResponse<Game>
            {
                Data = game
            });
        }

        [HttpDelete("delete/{challengerId}/{recipientId}")]
        public ActionResult<ApiMethodResponse<bool>> DeleteChallenge(int challengerId, int recipientId)
        {
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

        private Game CreateNewGame(ChallengeDatabaseModel challenge, User challenger, User recipient)
        {
            var game = new Game();
            var isChallengerWhite = GetChallengerColor(challenge);

            game.WhitePlayer = isChallengerWhite ? challenger : recipient;
            game.BlackPlayer = isChallengerWhite ? recipient : challenger;

            var newGameId = _stringIdGenerator.GenerateId();
            _gameRepository.CreateGame(newGameId, game.WhitePlayer.Id, game.BlackPlayer.Id);

            game.Id = newGameId;

            return game;
        }

        private bool GetChallengerColor(ChallengeDatabaseModel challenge)
        {
            bool isChallengerWhite;
            if (challenge.ChallengerColor == ChallengerColor.Random)
            {
                isChallengerWhite = _random.NextDouble() > 0.5;
            }
            else
            {
                isChallengerWhite = challenge.ChallengerColor == ChallengerColor.White;
            }

            return isChallengerWhite;
        }
    }
}
