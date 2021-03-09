using System;
using Chess.Api.Repositories.Interfaces;
using Chess.Api.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Chess.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthcheckController : Controller
    {
        private readonly IChallengeRepository _challengeRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IUserRepository _userRepository;

        public HealthcheckController(IChallengeRepository challengeRepository, IGameRepository gameRepository,
            IUserRepository userRepository)
        {
            _challengeRepository = challengeRepository;
            _gameRepository = gameRepository;
            _userRepository = userRepository;
        }

        [HttpGet("challenges")]
        public ActionResult<ApiMethodResponse<bool>> Challenges()
        {
            return HealthCheck(_challengeRepository);
        }

        [HttpGet("games")]
        public ActionResult<ApiMethodResponse<bool>> Games()
        {
            return HealthCheck(_gameRepository);
        }

        [HttpGet("users")]
        public ActionResult<ApiMethodResponse<bool>> Users()
        {
            return HealthCheck(_userRepository);
        }

        private ObjectResult HealthCheck(IHasHealthCheck healthCheck)
        {
            try
            {
                var healthy = healthCheck.CheckHealth();

                var response = new ApiMethodResponse<bool>()
                {
                    Data = healthy
                };

                if (healthy)
                {
                    return Ok(response);
                }

                return StatusCode(500, response);
            }
            catch (Exception e)
            {
                var response = new ApiMethodResponse<bool>()
                {
                    Data = false,
                    Errors = new[] { e.Message }
                };

                return StatusCode(500, response);
            }
        }
    }
}
