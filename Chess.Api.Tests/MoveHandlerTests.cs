using Chess.Api.MoveValidation;
using Chess.Api.MoveValidation.Interfaces;
using FluentAssertions;
using NUnit.Framework;

namespace Chess.Api.Tests
{
    public class MoveHandlerTests
    {
        private readonly IMoveHandler _moveHandler;
        private readonly CoordinateNotationParser _coordinateNotationParser;
        private readonly FenParser _fenParser;

        public MoveHandlerTests()
        {
            _moveHandler = new MoveHandler();
            _coordinateNotationParser = new CoordinateNotationParser();
            _fenParser = new FenParser();
        }

        [Test]
        public void ApplyMove_ShouldIncrementHalfmoveClock_WhenNotAPawnOrCaptureMove()
        {
            var fen = "8/8/8/3Q4/8/8/8/8 w - - 0 1";
            var moveNotation = "d5e4";
            var move = _coordinateNotationParser.ParseNotationMove(moveNotation);
            var boardState = _fenParser.ParseFen(fen);
            var moveValidationResult = new MoveValidationResult
            {
                IsValid = true,
                ShouldResetHalfmoveClock = false
            };

            var result = _moveHandler.ApplyMove(boardState, move, moveValidationResult);

            result.HalfmoveClock.Should().Be(1);
        }

        [Test]
        public void ApplyMove_ShouldResetHalfmoveClock_WhenPawnOrCaptureMove()
        {
            var fen = "8/8/8/3P4/8/8/8/8 w - - 10 1";
            var moveNotation = "d5d6";
            var move = _coordinateNotationParser.ParseNotationMove(moveNotation);
            var boardState = _fenParser.ParseFen(fen);
            var moveValidationResult = new MoveValidationResult
            {
                IsValid = true,
                ShouldResetHalfmoveClock = true
            };

            var result = _moveHandler.ApplyMove(boardState, move, moveValidationResult);

            result.HalfmoveClock.Should().Be(0);
        }
    }
}
