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
        private PostUserModelValidator _postUserModelValidator;
        private ILogger<UsersController> _logger;

        public UsersController(IUserRepository userRepository, ILogger<UsersController> logger)
        {
            _userRepository = userRepository;
            _postUserModelValidator = new PostUserModelValidator();
            _logger = logger;
        }

        // POST api/<controller>
        [HttpPost]
        public ActionResult<ApiMethodResponse<int>> Post([FromBody]PostUserModel userModel)
        {
            var validationResult = _postUserModelValidator.Validate(userModel);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiMethodResponse<int>
                {
                    Errors = validationResult.Errors.Select(error => error.ToString()).ToArray()
                });
            }

            var salt = GenerateRandomSalt();
            var hashedPassword = HashPassword(userModel.Password, salt);

            var newId = _userRepository.CreateUser(userModel.Username, hashedPassword, Convert.ToBase64String(salt));
            _logger.LogInformation($"Created new user. Username {userModel.Username} ID {newId}");

            return Ok(new ApiMethodResponse<int>
            {
                Data = newId
            });
        }

        private byte[] GenerateRandomSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private string HashPassword(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));
        }
    }
}
