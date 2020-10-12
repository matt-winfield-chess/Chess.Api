using Chess.Api.Controllers;
using Chess.Api.Interfaces.Repositories;
using Chess.Api.Models;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Chess.Api.Responses;

namespace Chess.Api.Tests
{
    public class UsersControllerTests
    {
        private const string VALID_USERNAME = "ValidUsername";
        private const string VALID_PASSWORD = "ValidPassword";
        private const string WHITESPACE = "      ";
        private const string NON_ASCII_STRING = "non-ascii 😨";
        private readonly int CREATED_USER_ID = 1;

        private UsersController _controller;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ILogger<UsersController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<UsersController>>();

            _controller = new UsersController(_userRepositoryMock.Object, _loggerMock.Object);
        }

        [Test]
        public void PostUser_ShouldReturnSuccess_WhenValidUsernameAndPassword()
        {
            _userRepositoryMock.Setup(mock => 
                mock.CreateUser(VALID_USERNAME, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(CREATED_USER_ID);

            var actionResult = _controller.Post(new PostUserModel
            {
                Username = VALID_USERNAME,
                Password = VALID_PASSWORD
            });

            var result = TestHelper.ConvertObjectResponse<ApiMethodResponse<int>, OkObjectResult>(actionResult);
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(CREATED_USER_ID);
        }

        [Test]
        public void PostUser_ShouldReturnError_WhenUsernameEmpty()
        {
            var actionResult = _controller.Post(new PostUserModel
            {
                Username = string.Empty,
                Password = VALID_PASSWORD
            });

            var result = TestHelper.ConvertObjectResponse<ApiMethodResponse<int>, BadRequestObjectResult>(actionResult);
            result.IsSuccess.Should().BeFalse();
            result.Errors.Length.Should().Be(1);
        }

        [Test]
        public void PostUser_ShouldReturnError_WhenPasswordEmpty()
        {
            var actionResult = _controller.Post(new PostUserModel
            {
                Username = VALID_USERNAME,
                Password = string.Empty
            });

            var result = TestHelper.ConvertObjectResponse<ApiMethodResponse<int>, BadRequestObjectResult>(actionResult);
            result.IsSuccess.Should().BeFalse();
            result.Errors.Length.Should().Be(1);
        }

        [Test]
        public void PostUser_ShouldReturnError_WhenUsernameAndPasswordEmpty()
        {
            var actionResult = _controller.Post(new PostUserModel
            {
                Username = string.Empty,
                Password = string.Empty
            });

            var result = TestHelper.ConvertObjectResponse<ApiMethodResponse<int>, BadRequestObjectResult>(actionResult);
            result.IsSuccess.Should().BeFalse();
            result.Errors.Length.Should().Be(2);
        }

        [Test]
        public void PostUser_ShouldReturnError_WhenUsernameWhitespace()
        {
            var actionResult = _controller.Post(new PostUserModel
            {
                Username = WHITESPACE,
                Password = VALID_PASSWORD
            });

            var result = TestHelper.ConvertObjectResponse<ApiMethodResponse<int>, BadRequestObjectResult>(actionResult);
            result.IsSuccess.Should().BeFalse();
            result.Errors.Length.Should().Be(1);
        }

        [Test]
        public void PostUser_ShouldReturnError_WhenPasswordWhitespace()
        {
            var actionResult = _controller.Post(new PostUserModel
            {
                Username = VALID_USERNAME,
                Password = WHITESPACE
            });

            var result = TestHelper.ConvertObjectResponse<ApiMethodResponse<int>, BadRequestObjectResult>(actionResult);
            result.IsSuccess.Should().BeFalse();
            result.Errors.Length.Should().Be(1);
        }

        [Test]
        public void PostUser_ShouldReturnError_WhenUsernameContainsNonAsciiCharacters()
        {
            var actionResult = _controller.Post(new PostUserModel
            {
                Username = NON_ASCII_STRING,
                Password = VALID_PASSWORD
            });

            var result = TestHelper.ConvertObjectResponse<ApiMethodResponse<int>, BadRequestObjectResult>(actionResult);
            result.IsSuccess.Should().BeFalse();
            result.Errors.Length.Should().Be(1);
        }

        [Test]
        public void PostUser_ShouldReturnError_WhenPasswordContainsNonAsciiCharacters()
        {
            var actionResult = _controller.Post(new PostUserModel
            {
                Username = VALID_USERNAME,
                Password = NON_ASCII_STRING
            });

            var result = TestHelper.ConvertObjectResponse<ApiMethodResponse<int>, BadRequestObjectResult>(actionResult);
            result.IsSuccess.Should().BeFalse();
            result.Errors.Length.Should().Be(1);
        }
    }
}