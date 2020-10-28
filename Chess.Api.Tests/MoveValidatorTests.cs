using Chess.Api.MoveValidation;
using Chess.Api.MoveValidation.Interfaces;
using FluentAssertions;
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

        [TestCase("8/8/8/8/8/8/P7/8 w KQkq - 0 1", "a2a3", "white pawn should be able to move 1 space")]
        [TestCase("8/8/8/8/8/8/P7/8 w KQkq - 0 1", "a2a4", "white pawn should be able to move 2 squares from starting position")]
        [TestCase("8/p7/8/8/8/8/8/8 b KQkq - 0 1", "a7a6", "black pawn should be able to move 1 space")]
        [TestCase("8/p7/8/8/8/8/8/8 b KQkq - 0 1", "a7a5", "black pawn should be able to move 2 squares from starting position")]
        public void MoveValidator_ShouldReturnValidMove_WhenPawnPerformsValidMove(string startingFen, string move, string because)
        {
            var result = _moveValidator.ValidateMove(startingFen, move);

            result.IsValid.Should().BeTrue(because);
        }

        [TestCase("8/8/8/4R3/8/8/8/8 w KQkq - 0 1", "e5d5", "rook should be able to move horizontally")]
        [TestCase("8/8/8/4R3/8/8/8/8 w KQkq - 0 1", "e5a5", "rook should be able to move horizontally")]
        [TestCase("8/8/8/4R3/8/8/8/8 w KQkq - 0 1", "e5f5", "rook should be able to move horizontally")]
        [TestCase("8/8/8/4R3/8/8/8/8 w KQkq - 0 1", "e5h5", "rook should be able to move horizontally")]
        [TestCase("8/8/8/4R3/8/8/8/8 w KQkq - 0 1", "e5e6", "rook should be able to move vertically")]
        [TestCase("8/8/8/4R3/8/8/8/8 w KQkq - 0 1", "e5e8", "rook should be able to move vertically")]
        [TestCase("8/8/8/4R3/8/8/8/8 w KQkq - 0 1", "e5e4", "rook should be able to move vertically")]
        [TestCase("8/8/8/4R3/8/8/8/8 w KQkq - 0 1", "e5e1", "rook should be able to move vertically")]
        public void MoveValidator_ShouldReturnValidMove_WhenRookPerformsValidMove(string startingFen, string move, string because)
        {
            var result = _moveValidator.ValidateMove(startingFen, move);

            result.IsValid.Should().BeTrue(because);
        }

        [TestCase("8/8/8/4Q3/8/8/8/8 w KQkq - 0 1", "e5d5", "queen should be able to move horizontally")]
        [TestCase("8/8/8/4Q3/8/8/8/8 w KQkq - 0 1", "e5a5", "queen should be able to move horizontally")]
        [TestCase("8/8/8/4Q3/8/8/8/8 w KQkq - 0 1", "e5f5", "queen should be able to move horizontally")]
        [TestCase("8/8/8/4Q3/8/8/8/8 w KQkq - 0 1", "e5h5", "queen should be able to move horizontally")]
        [TestCase("8/8/8/4Q3/8/8/8/8 w KQkq - 0 1", "e5e6", "queen should be able to move vertically")]
        [TestCase("8/8/8/4Q3/8/8/8/8 w KQkq - 0 1", "e5e8", "queen should be able to move vertically")]
        [TestCase("8/8/8/4Q3/8/8/8/8 w KQkq - 0 1", "e5e4", "queen should be able to move vertically")]
        [TestCase("8/8/8/4Q3/8/8/8/8 w KQkq - 0 1", "e5e1", "queen should be able to move vertically")]
        public void MoveValidator_ShouldReturnValidMove_WhenQueenPerformsValidMove(string startingFen, string move, string because)
        {
            var result = _moveValidator.ValidateMove(startingFen, move);

            result.IsValid.Should().BeTrue(because);
        }
    }
}
