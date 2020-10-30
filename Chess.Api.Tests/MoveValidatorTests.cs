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
        [TestCase("8/8/8/4Q3/8/8/8/8 w KQkq - 0 1", "e5d6", "queen should be able to move diagonally")]
        [TestCase("8/8/8/4Q3/8/8/8/8 w KQkq - 0 1", "e5b8", "queen should be able to move diagonally")]
        [TestCase("8/8/8/4Q3/8/8/8/8 w KQkq - 0 1", "e5f6", "queen should be able to move diagonally")]
        [TestCase("8/8/8/4Q3/8/8/8/8 w KQkq - 0 1", "e5h8", "queen should be able to move diagonally")]
        [TestCase("8/8/8/4Q3/8/8/8/8 w KQkq - 0 1", "e5f4", "queen should be able to move diagonally")]
        [TestCase("8/8/8/4Q3/8/8/8/8 w KQkq - 0 1", "e5h2", "queen should be able to move diagonally")]
        [TestCase("8/8/8/4Q3/8/8/8/8 w KQkq - 0 1", "e5d4", "queen should be able to move diagonally")]
        [TestCase("8/8/8/4Q3/8/8/8/8 w KQkq - 0 1", "e5a1", "queen should be able to move diagonally")]
        public void MoveValidator_ShouldReturnValidMove_WhenQueenPerformsValidMove(string startingFen, string move, string because)
        {
            var result = _moveValidator.ValidateMove(startingFen, move);

            result.IsValid.Should().BeTrue(because);
        }

        [TestCase("8/8/8/4K3/8/8/8/8 w KQkq - 0 1", "e5e6", "king should be able to move one square up")]
        [TestCase("8/8/8/4K3/8/8/8/8 w KQkq - 0 1", "e5d5", "king should be able to move one square right")]
        [TestCase("8/8/8/4K3/8/8/8/8 w KQkq - 0 1", "e5f5", "king should be able to move one square left")]
        [TestCase("8/8/8/4K3/8/8/8/8 w KQkq - 0 1", "e5e4", "king should be able to move one square down")]
        [TestCase("8/8/8/4K3/8/8/8/8 w KQkq - 0 1", "e5d6", "king should be able to move one square diagonally")]
        [TestCase("8/8/8/4K3/8/8/8/8 w KQkq - 0 1", "e5f6", "king should be able to move one square diagonally")]
        [TestCase("8/8/8/4K3/8/8/8/8 w KQkq - 0 1", "e5d4", "king should be able to move one square diagonally")]
        [TestCase("8/8/8/4K3/8/8/8/8 w KQkq - 0 1", "e5f4", "king should be able to move one square diagonally")]
        public void MoveValidator_ShouldReturnValidMove_WhenKingPerformsValidMove(string startingFen, string move, string because)
        {
            var result = _moveValidator.ValidateMove(startingFen, move);

            result.IsValid.Should().BeTrue(because);
        }

        [TestCase("8/8/8/4B3/8/8/8/8 w KQkq - 0 1", "e5d6", "bishop should be able to move diagonally")]
        [TestCase("8/8/8/4B3/8/8/8/8 w KQkq - 0 1", "e5b8", "bishop should be able to move diagonally")]
        [TestCase("8/8/8/4B3/8/8/8/8 w KQkq - 0 1", "e5f6", "bishop should be able to move diagonally")]
        [TestCase("8/8/8/4B3/8/8/8/8 w KQkq - 0 1", "e5h8", "bishop should be able to move diagonally")]
        [TestCase("8/8/8/4B3/8/8/8/8 w KQkq - 0 1", "e5f4", "bishop should be able to move diagonally")]
        [TestCase("8/8/8/4B3/8/8/8/8 w KQkq - 0 1", "e5h2", "bishop should be able to move diagonally")]
        [TestCase("8/8/8/4B3/8/8/8/8 w KQkq - 0 1", "e5d4", "bishop should be able to move diagonally")]
        [TestCase("8/8/8/4B3/8/8/8/8 w KQkq - 0 1", "e5a1", "bishop should be able to move diagonally")]
        public void MoveValidator_ShouldReturnValidMove_WhenBishopPerformsValidMove(string startingFen, string move, string because)
        {
            var result = _moveValidator.ValidateMove(startingFen, move);

            result.IsValid.Should().BeTrue(because);
        }

        [TestCase("8/8/8/4N3/8/8/8/8 w KQkq - 0 1", "e5g6", "knight should be able to move 2 squares in the x axis and 1 in the y axis")]
        [TestCase("8/8/8/4N3/8/8/8/8 w KQkq - 0 1", "e5g4", "knight should be able to move 2 squares in the x axis and 1 in the y axis")]
        [TestCase("8/8/8/4N3/8/8/8/8 w KQkq - 0 1", "e5c6", "knight should be able to move 2 squares in the x axis and 1 in the y axis")]
        [TestCase("8/8/8/4N3/8/8/8/8 w KQkq - 0 1", "e5c4", "knight should be able to move 2 squares in the x axis and 1 in the y axis")]
        [TestCase("8/8/8/4N3/8/8/8/8 w KQkq - 0 1", "e5f7", "knight should be able to move 2 squares in the y axis and 1 in the x axis")]
        [TestCase("8/8/8/4N3/8/8/8/8 w KQkq - 0 1", "e5f3", "knight should be able to move 2 squares in the y axis and 1 in the x axis")]
        [TestCase("8/8/8/4N3/8/8/8/8 w KQkq - 0 1", "e5d7", "knight should be able to move 2 squares in the y axis and 1 in the x axis")]
        [TestCase("8/8/8/4N3/8/8/8/8 w KQkq - 0 1", "e5d3", "knight should be able to move 2 squares in the y axis and 1 in the x axis")]
        public void MoveValidator_ShouldReturnValidMove_WhenKnightPerformsValidMove(string startingFen, string move, string because)
        {
            var result = _moveValidator.ValidateMove(startingFen, move);

            result.IsValid.Should().BeTrue(because);
        }

        [TestCase("8/8/8/8/8/8/P7/8 w KQkq - 0 1", "a2a4", 0, 2, "white pawn moving 2 squares should set target square")]
        [TestCase("8/8/8/8/8/8/1P6/8 w KQkq - 0 1", "b2b4", 1, 2, "white pawn moving 2 squares should set target square")]
        [TestCase("8/8/8/8/8/8/7P/8 w KQkq - 0 1", "h2h4", 7, 2, "white pawn moving 2 squares should set target square")]
        [TestCase("8/p7/8/8/8/8/8/8 b KQkq - 0 1", "a7a5", 0, 5, "black pawn moving 2 squares should set target square")]
        [TestCase("8/1p6/8/8/8/8/8/8 b KQkq - 0 1", "b7b5", 1, 5, "black pawn moving 2 squares should set target square")]
        [TestCase("8/7p/8/8/8/8/8/8 b KQkq - 0 1", "h7h5", 7, 5, "black pawn moving 2 squares should set target square")]
        public void MoveValidator_ShouldReturnEnPassantTargetSqaure_WhenPawnMovesTwoSquares(string fen, string move, int expectedX, int expectedY, string because)
        {
            var result = _moveValidator.ValidateMove(fen, move);

            result.NewEnPassantTarget.X.Should().Be(expectedX, because);
            result.NewEnPassantTarget.Y.Should().Be(expectedY, because);
        }

        [TestCase("8/8/8/5Pp1/8/8/8/8 w KQkq g6 0 1", "f5g6")]
        [TestCase("8/8/8/8/1Pp5/8/8/8 b KQkq b3 0 1", "c4b3")]
        public void MoveValidator_ShouldAllowEnPassantCapture_WhenEnPassantIsAvailable(string fen, string move)
        {
            var result = _moveValidator.ValidateMove(fen, move);

            result.IsValid.Should().BeTrue();
        }

        [TestCase("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1", "e1g1", 7, 0, 5, 0)]
        [TestCase("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1", "e1c1", 0, 0, 3, 0)]
        [TestCase("r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1", "e8g8", 7, 7, 5, 7)]
        [TestCase("r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1", "e8c8", 0, 7, 3, 7)]
        public void MoveValidator_ShouldAllowCastling_AndSetCastleMove(string fen, string move, int rookStartX, int rookStartY, int rookEndX, int rookEndY)
        {
            var result = _moveValidator.ValidateMove(fen, move);

            result.IsValid.Should().BeTrue();
            result.CastleRookMove.Should().NotBeNull();
            result.CastleRookMove.StartPosition.X.Should().Be(rookStartX);
            result.CastleRookMove.StartPosition.Y.Should().Be(rookStartY);
            result.CastleRookMove.EndPosition.X.Should().Be(rookEndX);
            result.CastleRookMove.EndPosition.Y.Should().Be(rookEndY);
        }

        [TestCase("8/8/8/8/8/8/6p1/8 b - - 0 1", "g2g1")]
        [TestCase("8/6P1/8/8/8/8/8/8 w - - 0 1", "g7g8")]
        public void MoveValidator_ShouldSetPromotion_WhenPawnPromoted(string fen, string move)
        {
            var result = _moveValidator.ValidateMove(fen, move);

            result.IsPromotion.Should().BeTrue();
        }

        [TestCase("8/8/8/4q3/3P4/8/8/8 w - - 0 1", "d4e5")]
        [TestCase("8/8/8/4p3/3Q4/8/8/8 b - - 0 1", "e5d4")]
        public void MoveValidator_ShouldReturnValid_WhenPawnCaptures(string fen, string move)
        {
            var result = _moveValidator.ValidateMove(fen, move);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void MoveValidator_ShouldReturnInvalid_WhenStartSquareDoesntHavePiece()
        {
            var fen = "8/8/8/8/8/8/8/8 w - - 0 1";
            var move = "e2e4";

            var result = _moveValidator.ValidateMove(fen, move);

            result.IsValid.Should().BeFalse();
        }

        [TestCase("8/8/8/8/8/8/P7/8 w - - 0 1", "a2a5")]
        [TestCase("8/8/8/8/8/8/P7/8 w - - 0 1", "a2a1")]
        [TestCase("8/8/8/8/8/8/P7/8 w - - 0 1", "a2b2")]
        [TestCase("8/8/8/8/8/8/P7/8 w - - 0 1", "a2b3")]
        [TestCase("8/8/8/8/8/8/P7/8 w - - 0 1", "a2b4")]
        [TestCase("8/8/8/8/8/8/P7/8 w - b6 0 1", "a2b6")]
        public void MoveValidator_ShouldReturnInvalid_WhenPawnTriesToMoveIllegalSquare(string fen, string move)
        {
            var result = _moveValidator.ValidateMove(fen, move);

            result.IsValid.Should().BeFalse();
        }

        [TestCase("8/p7/P7/8/8/8/8/8 b - - 0 1", "a7a6")]
        [TestCase("8/8/8/8/8/p7/P7/8 w - - 0 1", "a2a3")]
        public void MoveValidator_ShouldReturnInvalid_WhenPawnTriesToCaptureInFront(string fen, string move)
        {
            var result = _moveValidator.ValidateMove(fen, move);

            result.IsValid.Should().BeFalse();
        }

        [TestCase("8/8/8/8/8/p7/P7/8 w - - 0 1", "a2a4")]
        [TestCase("8/p7/P7/8/8/8/8/8 w - - 0 1", "a7a5")]
        public void MoveValidator_ShouldReturnInvalid_WhenPawnTriesToMoveToBlockedSquare(string fen, string move)
        {
            var result = _moveValidator.ValidateMove(fen, move);

            result.IsValid.Should().BeFalse();
        }

        [TestCase("8/8/8/3Rp3/8/8/8/8 w - - 0 1", "d5f5")]
        [TestCase("8/8/8/3Rp3/8/8/8/8 w - - 0 1", "d5h5")]
        [TestCase("8/8/8/3R1p2/8/8/8/8 w - - 0 1", "d5g5")]
        [TestCase("8/8/3p4/3R4/8/8/8/8 w - - 0 1", "d5d7")]
        [TestCase("8/8/3p4/3R4/8/8/8/8 w - - 0 1", "d5d8")]
        [TestCase("8/3p4/8/3R4/8/8/8/8 w - - 0 1", "d5d8")]
        [TestCase("8/8/8/2pR4/8/8/8/8 w - - 0 1", "d5b5")]
        [TestCase("8/8/8/2pR4/8/8/8/8 w - - 0 1", "d5a5")]
        [TestCase("8/8/8/3R4/3p4/8/8/8 w - - 0 1", "d5d3")]
        [TestCase("8/8/8/3R4/3p4/8/8/8 w - - 0 1", "d5d2")]
        [TestCase("8/8/8/3R4/8/3p4/8/8 w - - 0 1", "d5d2")]
        public void MoveValidator_ShouldReturnInvalid_WhenRookTriesToMoveToBlockedSquare(string fen, string move)
        {
            var result = _moveValidator.ValidateMove(fen, move);

            result.IsValid.Should().BeFalse();
        }

        [TestCase("8/8/8/3Qp3/8/8/8/8 w - - 0 1", "d5f5")]
        [TestCase("8/8/8/3Qp3/8/8/8/8 w - - 0 1", "d5h5")]
        [TestCase("8/8/8/3Q1p2/8/8/8/8 w - - 0 1", "d5g5")]
        [TestCase("8/8/3p4/3Q4/8/8/8/8 w - - 0 1", "d5d7")]
        [TestCase("8/8/3p4/3Q4/8/8/8/8 w - - 0 1", "d5d8")]
        [TestCase("8/3p4/8/3Q4/8/8/8/8 w - - 0 1", "d5d8")]
        [TestCase("8/8/8/2pQ4/8/8/8/8 w - - 0 1", "d5b5")]
        [TestCase("8/8/8/2pQ4/8/8/8/8 w - - 0 1", "d5a5")]
        [TestCase("8/8/8/3Q4/3p4/8/8/8 w - - 0 1", "d5d3")]
        [TestCase("8/8/8/3Q4/3p4/8/8/8 w - - 0 1", "d5d2")]
        [TestCase("8/8/8/3Q4/8/3p4/8/8 w - - 0 1", "d5d2")]
        [TestCase("8/1p6/8/3Q4/8/8/8/8 w - - 0 1", "d5a8")]
        [TestCase("8/5p2/8/3Q4/8/8/8/8 w - - 0 1", "d5g8")]
        [TestCase("8/8/8/3Q4/8/5p2/8/8 w - - 0 1", "d5g2")]
        [TestCase("8/8/8/3Q4/8/1p6/8/8 w - - 0 1", "d5a2")]
        [TestCase("8/8/8/3QP3/8/8/8/8 w - - 0 1", "d5e5")]
        public void MoveValidator_ShouldReturnInvalid_WhenQueenTriesToMoveToBlockedSquare(string fen, string move)
        {
            var result = _moveValidator.ValidateMove(fen, move);

            result.IsValid.Should().BeFalse();
        }

        [TestCase("8/8/8/3KP3/8/8/8/8 w - - 0 1", "d5e5")]
        [TestCase("8/8/8/3K4/4P3/8/8/8 w - - 0 1", "d5e4")]
        [TestCase("8/8/8/3K4/3P4/8/8/8 w - - 0 1", "d5d4")]
        [TestCase("8/8/8/3K4/2P5/8/8/8 w - - 0 1", "d5c4")]
        [TestCase("8/8/8/2PK4/8/8/8/8 w - - 0 1", "d5c5")]
        [TestCase("8/8/2P5/3K4/8/8/8/8 w - - 0 1", "d5c6")]
        [TestCase("8/8/3P4/3K4/8/8/8/8 w - - 0 1", "d5d6")]
        [TestCase("8/8/4P3/3K4/8/8/8/8 w - - 0 1", "d5e6")]
        public void MoveValidator_ShouldReturnInvalid_WhenKingTriesToMoveToBlockedSquare(string fen, string move)
        {
            var result = _moveValidator.ValidateMove(fen, move);

            result.IsValid.Should().BeFalse();
        }

        [TestCase("8/8/3q4/8/3B4/3K4/8/8 w - - 0 1", "d4e5", "moving bishop discovers vertical check by queen")]
        [TestCase("8/8/8/3qpPK1/8/8/8/8 w - e6 0 1", "f5e6", "en-passant capture discovers horizontal check by queen")]
        [TestCase("8/5q2/4B3/3K4/8/8/8/8 w - - 0 1", "e6d7", "moving bishop discovers diagonal check by queen")]
        [TestCase("8/8/8/3KBq2/8/8/8/8 w - - 0 1", "e5d6", "moving bishop discovers horizontal check by queen")]
        [TestCase("8/8/8/3K1q2/8/6B1/8/8 w - - 0 1", "g3f2", "moving the bishop to this square does not block the check by the queen")]
        [TestCase("8/8/8/3K1k2/8/8/8/8 w - - 0 1", "d5e5", "the king cannot be moved to this square because it is controlled by the opponent king")]
        [TestCase("8/4p3/8/4K3/8/8/8/8 w - - 0 1", "e5d6", "the king cannot move to this square because the pawn controls it")]
        [TestCase("8/4p3/8/4K3/8/8/8/8 w - - 0 1", "e5f6", "the king cannot move to this square because the pawn controls it")]
        [TestCase("8/8/8/2K1n3/8/8/8/8 w - - 0 1", "c5c6", "the king cannot move to this square because the knight controls it")]
        [TestCase("8/8/8/2K1n3/8/8/8/8 w - - 0 1", "c5c4", "the king cannot move to this square because the knight controls it")]
        [TestCase("8/8/4K3/4n3/8/8/8/8 w - - 0 1", "e6d7", "the king cannot move to this square because the knight controls it")]
        [TestCase("8/8/4K3/4n3/8/8/8/8 w - - 0 1", "e6f7", "the king cannot move to this square because the knight controls it")]
        public void MoveValidator_ShouldRetrunInvalid_WhenMoveWouldPlaceOwnKingInCheck(string fen, string move, string because)
        {
            var result = _moveValidator.ValidateMove(fen, move);

            result.IsValid.Should().BeFalse(because);
        }
    }
}
