using Chess.Api.Authentication;
using Chess.Api.Interfaces.Repositories;
using Chess.Api.Models;
using Chess.Api.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Chess.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {
        private IUserRepository _userRepository;
        private ICredentialService _credentialService;
        private ILogger<AuthenticationController> _logger;

        public AuthenticationController(IUserRepository userRepository, ICredentialService credentialService, ILogger<AuthenticationController> logger)
        {
            _userRepository = userRepository;
            _credentialService = credentialService;
            _logger = logger;
        }

        [HttpPost("Authenticate")]
        public ActionResult<ApiMethodResponse<string>> Post([FromBody] PostCredentialsModel credentials)
        {
            var credentialsToCompare = _userRepository.GetUserCredentialsByUsername(credentials.Username);

            if (credentialsToCompare == null)
            {
                _logger.LogInformation($"Failed to get login details for '{credentials.Username}'");
                return Unauthorized(new ApiMethodResponse<string>
                {
                    Errors = new string[] { "Username or password was incorrect" }
                });
            }

            var salt = Convert.FromBase64String(credentialsToCompare.Salt);
            var passwordHash = _credentialService.HashPassword(credentials.Password, salt);

            if (!passwordHash.Equals(credentialsToCompare.HashedPassword))
            {
                _logger.LogInformation($"Login failed for user {credentialsToCompare.UserId} ('{credentials.Username}')");
                return Unauthorized(new ApiMethodResponse<string>
                {
                    Errors = new string[] { "Username or password was incorrect" }
                });
            }

            return Ok(new ApiMethodResponse<string>());
        }
    }
}
