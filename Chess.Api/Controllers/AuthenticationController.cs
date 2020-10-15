using Chess.Api.Authentication.Interfaces;
using Chess.Api.Models;
using Chess.Api.Repositories.Interfaces;
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
        private IJwtService _jwtService;
        private ILogger<AuthenticationController> _logger;

        public AuthenticationController(IUserRepository userRepository, ICredentialService credentialService, IJwtService jwtService, ILogger<AuthenticationController> logger)
        {
            _userRepository = userRepository;
            _credentialService = credentialService;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("Authenticate")]
        public ActionResult<ApiMethodResponse<LoginResponse>> Post([FromBody] PostCredentialsModel credentials)
        {
            var credentialsToCompare = _userRepository.GetUserCredentialsByUsername(credentials.Username);

            if (credentialsToCompare == null)
            {
                _logger.LogInformation($"Failed to get login details for '{credentials.Username}'");
                return Unauthorized(new ApiMethodResponse<LoginResponse>
                {
                    Errors = new string[] { "Username or password was incorrect" }
                });
            }

            var salt = Convert.FromBase64String(credentialsToCompare.Salt);
            var passwordHash = _credentialService.HashPassword(credentials.Password, salt);

            if (!passwordHash.Equals(credentialsToCompare.HashedPassword))
            {
                _logger.LogInformation($"Login failed for user {credentialsToCompare.UserId} ('{credentials.Username}')");
                return Unauthorized(new ApiMethodResponse<LoginResponse>
                {
                    Errors = new string[] { "Username or password was incorrect" }
                });
            }

            var token = _jwtService.GenerateJwtToken(credentialsToCompare.UserId);

            return Ok(new ApiMethodResponse<LoginResponse>
            {
                Data = new LoginResponse
                {
                    Id = credentialsToCompare.UserId,
                    Username = credentials.Username,
                    Token = token
                }
            });
        }
    }
}
