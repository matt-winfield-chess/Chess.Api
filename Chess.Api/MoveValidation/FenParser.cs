using System;
using System.Collections.Generic;

namespace Chess.Api.MoveValidation
{
    public class FenParser
    {
        private Dictionary<char, PieceType> _pieceTypeCharacterMap = new Dictionary<char, PieceType>
        {
            {'p', PieceType.Pawn},
            {'n', PieceType.Knight},
            {'b', PieceType.Bishop},
            {'r', PieceType.Rook},
            {'q', PieceType.Queen},
            {'k', PieceType.King}
        };

        public BoardState ParseFen(string fen)
        {
            var fenComponents = fen.Split(' ');
            if (fenComponents.Length != 6)
            {
                throw new ArgumentException("Invalid FEN string");
            }

            var state = new BoardState()
            {
                PiecePositions = ParsePieces(fenComponents[0]),
                ActiveColor = ParseActiveColor(fenComponents[1]),
                CastlingState = ParseCastlingState(fenComponents[2]),
                EnPassantTarget = ParseEnPassantTarget(fenComponents[3]),
                HalfmoveClock = ParseNumberComponent(fenComponents[4]),
                FullmoveNumber = ParseNumberComponent(fenComponents[5])
            };

            return state;
        }

        private Piece[,] ParsePieces(string position)
        {
            var piecePositions = new Piece[8, 8];

            var x = 0;
            var y = 0;

            foreach (char character in position)
            {
                if (char.IsDigit(character))
                {
                    x += (int) char.GetNumericValue(character);
                } else if (character == '/')
                {
                    y += 1;
                    x = 0;
                }
                else
                {
                    piecePositions[x, y] = GetPieceFromCharacter(character);
                    x += 1;
                }
            }

            return piecePositions;
        }

        private Color ParseActiveColor(string activeColorSection)
        {
            return activeColorSection[0] == 'w' ? Color.White : Color.Black;
        }

        private CastlingState ParseCastlingState(string castleStateSection)
        {
            return new CastlingState
            {
                WhiteKingside = castleStateSection.Contains('K'),
                WhiteQueenside = castleStateSection.Contains('Q'),
                BlackKingside = castleStateSection.Contains('k'),
                BlackQueenside = castleStateSection.Contains('q'),
            };
        }

        private Coordinate ParseEnPassantTarget(string enPassantString)
        {
            var parser = new CoordinateNotationParser();
            return parser.ParseNotationCoordinate(enPassantString);
        }

        private int ParseNumberComponent(string component)
        {
            return int.Parse(component);
        }

        private Piece GetPieceFromCharacter(char character)
        {
            return new Piece()
            {
                Color = char.IsUpper(character) ? Color.White : Color.Black,
                Type = _pieceTypeCharacterMap[char.ToLower(character)]
            };
        }
    }
}
