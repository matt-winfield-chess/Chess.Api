using Chess.Api.MoveValidation;
using Chess.Api.MoveValidation.Interfaces;
using NUnit.Framework;

namespace Chess.Api.Tests
{
    public class MoveValidatorTests
    {
        private IMoveValidator _moveValidator;

        [SetUp]
        public void Setup()
        {
            _moveValidator = new MoveValidator();
        }
    }
}
