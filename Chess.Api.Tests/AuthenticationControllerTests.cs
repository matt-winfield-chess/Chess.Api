using Chess.Api.Authentication.Interfaces;
using Chess.Api.Controllers;
using Chess.Api.Interfaces.Repositories;
using Chess.Api.Models;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using Chess.Api.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Chess.Api.Tests
{
    public class AuthenticationControllerTests
    {
        private const string VALID_USERNAME = "ValidUsername";
        private const string VALID_PASSWORD = "ValidPassword";
        private const string INVALID_PASSWORD = "InvalidPassword";

        private readonly HashedCredentials VALID_HASHED_CREDENTIALS = new HashedCredentials
        {
            UserId = 1,
            HashedPassword = "HashedPassword",
            Salt = "Salt"
        };
        private const string INVALID_PASSWORD_HASH = "InvalidHash";
        private readonly byte[] VALID_SALT = new byte[] { 0, 0, 0, 0 };
        private const string VALID_JWT = "ValidJwt";

        private AuthenticationController _controller;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ICredentialService> _credentialServiceMock;
        private Mock<IJwtService> _jwtServiceMock;
        private Mock<ILogger<AuthenticationController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _credentialServiceMock = new Mock<ICredentialService>();
            _jwtServiceMock = new Mock<IJwtService>();
            _loggerMock = new Mock<ILogger<AuthenticationController>>();

            _controller = new AuthenticationController(_userRepositoryMock.Object, _credentialServiceMock.Object, _jwtServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public void Authenticate_ShouldReturnJwt_WhenValidCredentials()
        {
            _userRepositoryMock.Setup(mock => mock.GetUserCredentialsByUsername(VALID_USERNAME))
                .Returns(VALID_HASHED_CREDENTIALS);

            _credentialServiceMock.Setup(mock => mock.GenerateRandomSalt())
                .Returns(VALID_SALT);
            _credentialServiceMock.Setup(mock => mock.HashPassword(VALID_PASSWORD, It.IsAny<byte[]>()))
                .Returns(VALID_HASHED_CREDENTIALS.HashedPassword);

            _jwtServiceMock.Setup(mock => mock.GenerateJwtToken(VALID_HASHED_CREDENTIALS.UserId))
                .Returns(VALID_JWT);

            var actionResult = _controller.Post(new PostCredentialsModel
            {
                Username = VALID_USERNAME,
                Password = VALID_PASSWORD
            });

            var result = TestHelper.ConvertObjectResponse<ApiMethodResponse<LoginResponse>, OkObjectResult>(actionResult);
            result.IsSuccess.Should().BeTrue();
            result.Data.Token.Should().Be(VALID_JWT);
        }

        [Test]
        public void Authenticate_ShouldReturnError_WhenUsernameDoesNotExist()
        {
            _userRepositoryMock.Setup(mock => mock.GetUserCredentialsByUsername(VALID_USERNAME))
                .Returns<HashedCredentials>(null);

            var actionResult = _controller.Post(new PostCredentialsModel
            {
                Username = VALID_USERNAME,
                Password = VALID_PASSWORD
            });

            var result = TestHelper.ConvertObjectResponse<ApiMethodResponse<LoginResponse>, UnauthorizedObjectResult>(actionResult);
            result.IsSuccess.Should().BeFalse();
            result.Errors.Length.Should().Be(1);
        }

        [Test]
        public void Authenticate_ShouldReturnError_WhenPasswordHashDoesNotMatch()
        {
            _userRepositoryMock.Setup(mock => mock.GetUserCredentialsByUsername(VALID_USERNAME))
                .Returns(VALID_HASHED_CREDENTIALS);

            _credentialServiceMock.Setup(mock => mock.GenerateRandomSalt())
                .Returns(VALID_SALT);
            _credentialServiceMock.Setup(mock => mock.HashPassword(INVALID_PASSWORD, It.IsAny<byte[]>()))
                .Returns(INVALID_PASSWORD_HASH);

            var actionResult = _controller.Post(new PostCredentialsModel
            {
                Username = VALID_USERNAME,
                Password = INVALID_PASSWORD
            });

            var result = TestHelper.ConvertObjectResponse<ApiMethodResponse<LoginResponse>, UnauthorizedObjectResult>(actionResult);
            result.IsSuccess.Should().BeFalse();
            result.Errors.Length.Should().Be(1);
        }
    }
}
