using Chess.Api.Authentication;
using Chess.Api.Interfaces.Repositories;
using Chess.Api.Models;
using Chess.Api.Responses;
using Chess.Api.Validators;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace Chess.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private IUserRepository _userRepository;
        private ICredentialService _credentialService;
        private PostUserModelValidator _postUserModelValidator;
        private ILogger<UsersController> _logger;

        public UsersController(IUserRepository userRepository, ICredentialService credentialService, ILogger<UsersController> logger)
        {
            _userRepository = userRepository;
            _credentialService = credentialService;
            _postUserModelValidator = new PostUserModelValidator();
            _logger = logger;
        }

        // POST api/<controller>
        [HttpPost]
        public ActionResult<ApiMethodResponse<int>> Post([FromBody] PostUserModel userModel)
        {
            var validationResult = _postUserModelValidator.Validate(userModel);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiMethodResponse<int>
                {
                    Errors = validationResult.Errors.Select(error => error.ToString()).ToArray()
                });
            }

            var salt = _credentialService.GenerateRandomSalt();
            var hashedPassword = _credentialService.HashPassword(userModel.Password, salt);

            try
            {
                var newId = _userRepository.CreateUser(userModel.Username, hashedPassword, Convert.ToBase64String(salt));
                _logger.LogInformation($"Created new user. Username {userModel.Username} ID {newId}");

                return Ok(new ApiMethodResponse<int>
                {
                    Data = newId
                });
            } catch (Exception e)
            {
                _logger.LogError($"User creation failed - {e.Message}");
                return BadRequest(new ApiMethodResponse<int>
                {
                    Errors = new string[] { "User account creation failed" }
                });
            }
        }
    }
}
