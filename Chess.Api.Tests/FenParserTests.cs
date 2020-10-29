using Chess.Api.MoveValidation;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;

namespace Chess.Api.Tests
{
    public class FenParserTests
    {
        private FenParser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new FenParser();
        }

        [TestCase("8/P7/8/8/8/8/8/8 w KQkq - 0 1", PieceType.Pawn)]
        [TestCase("8/B7/8/8/8/8/8/8 w KQkq - 0 1", PieceType.Bishop)]
        [TestCase("8/N7/8/8/8/8/8/8 w KQkq - 0 1", PieceType.Knight)]
        [TestCase("8/Q7/8/8/8/8/8/8 w KQkq - 0 1", PieceType.Queen)]
        [TestCase("8/K7/8/8/8/8/8/8 w KQkq - 0 1", PieceType.King)]
        [TestCase("8/R7/8/8/8/8/8/8 w KQkq - 0 1", PieceType.Rook)]
        public void ParseFen_ShouldParseWhitePieceInCorrectPosition_WhenFenContainsWhitePiece(string fen, PieceType expectedPieceType)
        {
            var result = _parser.ParseFen(fen);

            var piece = result.PiecePositions[0, 6];

            piece.Should().NotBeNull();
            piece.Color.Should().Be(Color.White);
            piece.Type.Should().Be(expectedPieceType);
        }

        [TestCase("8/p7/8/8/8/8/8/8 w KQkq - 0 1", PieceType.Pawn)]
        [TestCase("8/b7/8/8/8/8/8/8 w KQkq - 0 1", PieceType.Bishop)]
        [TestCase("8/n7/8/8/8/8/8/8 w KQkq - 0 1", PieceType.Knight)]
        [TestCase("8/q7/8/8/8/8/8/8 w KQkq - 0 1", PieceType.Queen)]
        [TestCase("8/k7/8/8/8/8/8/8 w KQkq - 0 1", PieceType.King)]
        [TestCase("8/r7/8/8/8/8/8/8 w KQkq - 0 1", PieceType.Rook)]
        public void ParseFen_ShouldParseBlackPieceInCorrectPosition_WhenFenContainsBlackPiece(string fen, PieceType expectedPieceType)
        {
            var result = _parser.ParseFen(fen);

            var piece = result.PiecePositions[0, 6];

            piece.Should().NotBeNull();
            piece.Color.Should().Be(Color.Black);
            piece.Type.Should().Be(expectedPieceType);
        }

        [TestCase("8/8/8/8/8/8/8/8 w KQkq - 0 1", Color.White)]
        [TestCase("8/8/8/8/8/8/8/8 b KQkq - 0 1", Color.Black)]
        public void ParseFen_ShouldParseActiveColor(string fen, Color expectedColor)
        {
            var result = _parser.ParseFen(fen);

            result.ActiveColor.Should().Be(expectedColor);
        }

        [TestCase("8/8/8/8/8/8/8/8 w KQkq - 0 1", true, true, true, true)]
        [TestCase("8/8/8/8/8/8/8/8 w K - 0 1", true, false, false, false)]
        [TestCase("8/8/8/8/8/8/8/8 w Q - 0 1", false, true, false, false)]
        [TestCase("8/8/8/8/8/8/8/8 w k - 0 1", false, false, true, false)]
        [TestCase("8/8/8/8/8/8/8/8 w q - 0 1", false, false, false, true)]
        public void ParseFen_ShouldParseCastlingState(string fen, bool whiteKingside, bool whiteQueenside, bool blackKingside, bool blackQueenside)
        {
            var result = _parser.ParseFen(fen);

            result.CastlingState.WhiteKingside.Should().Be(whiteKingside);
            result.CastlingState.WhiteQueenside.Should().Be(whiteQueenside);
            result.CastlingState.BlackKingside.Should().Be(blackKingside);
            result.CastlingState.BlackQueenside.Should().Be(blackQueenside);
        }

        [TestCase("8/8/8/8/8/8/8/8 w q e6 0 1", 4, 5)]
        [TestCase("8/8/8/8/8/8/8/8 w q d3 0 1", 3, 2)]
        public void ParseFen_ShouldParseCorrectEnPassantTargetSquare(string fen, int expectedX, int expectedY)
        {
            var result = _parser.ParseFen(fen);

            result.EnPassantTarget.X.Should().Be(expectedX);
            result.EnPassantTarget.Y.Should().Be(expectedY);
        }

        [Test]
        public void ParseFen_ShouldHaveNullEnPassantTarget_WhenNoEnPassantTargetInFen()
        {
            var fen = "8/8/8/8/8/8/8/8 w q - 0 1";

            var result = _parser.ParseFen(fen);

            result.EnPassantTarget.Should().BeNull("no en-passant target in fen string");
        }

        [Test]
        public void ParseFen_ShouldParseHalfmoveClock()
        {
            var fen = "8/8/8/8/8/8/8/8 w KQkq - 10 1";

            var result = _parser.ParseFen(fen);

            result.HalfmoveClock.Should().Be(10);
        }

        [Test]
        public void ParseFen_ShouldParseFullmoveNumber()
        {
            var fen = "8/8/8/8/8/8/8/8 w KQkq - 0 10";

            var result = _parser.ParseFen(fen);

            result.FullmoveNumber.Should().Be(10);
        }

        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w K - 0 1")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Q - 0 1")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w k - 0 1")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w q - 0 1")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Kq - 0 1")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w - - 0 1")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w - e3 0 1")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w - e3 10 1")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w - e3 0 10")]
        public void ConvertBoardStateToFen_ShouldCreateCorrectFen(string fen)
        {
            var boardState = _parser.ParseFen(fen);

            var result = _parser.ConvertBoardStateToFen(boardState);

            result.Should().Be(fen);
        }
    }
}
