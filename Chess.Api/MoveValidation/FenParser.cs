using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Api.MoveValidation
{
    public class FenParser
    {
        private readonly Dictionary<char, PieceType> _characterToPieceType = new Dictionary<char, PieceType>
        {
            {'p', PieceType.Pawn},
            {'n', PieceType.Knight},
            {'b', PieceType.Bishop},
            {'r', PieceType.Rook},
            {'q', PieceType.Queen},
            {'k', PieceType.King}
        };

        private readonly Dictionary<PieceType, char> _pieceTypeToCharacter;

        public FenParser()
        {
            // Invert the character-to-piece dictionary so it can be reversed;
            _pieceTypeToCharacter = _characterToPieceType.ToDictionary(dict => dict.Value, dict => dict.Key);
        }

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

        public string ConvertBoardStateToFen(BoardState state)
        {
            var pieces = GetPiecesFen(state.PiecePositions);
            var activeColor = state.ActiveColor == Color.White ? 'w' : 'b';
            var castlingState = GetCastlingStateFen(state.CastlingState);
            var enPassantTargetSquare = state.EnPassantTarget != null ? state.EnPassantTarget.ToString() : "-";

            return $"{pieces} {activeColor} {castlingState} {enPassantTargetSquare} {state.HalfmoveClock} {state.FullmoveNumber}";
        }

        private Piece[,] ParsePieces(string position)
        {
            var piecePositions = new Piece[8, 8];

            var x = 0;
            var y = 7;

            foreach (char character in position)
            {
                if (char.IsDigit(character))
                {
                    x += (int) char.GetNumericValue(character);
                } else if (character == '/')
                {
                    y -= 1;
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
                Type = _characterToPieceType[char.ToLower(character)]
            };
        }

        private string GetPiecesFen(Piece[,] piecePositions)
        {
            string output = "";
            int spaceCount = 0;
            for (int y = piecePositions.GetLength(1) - 1; y >= 0; y--)
            {
                for (int x = 0; x < piecePositions.GetLength(0); x++)
                {
                    var piece = piecePositions[x, y];
                    if (piece == null)
                    {
                        spaceCount += 1;
                    }
                    else
                    {
                        if (spaceCount > 0)
                        {
                            output += spaceCount.ToString();
                            spaceCount = 0;
                        }

                        char character = _pieceTypeToCharacter[piece.Type];

                        if (piece.Color == Color.White)
                        {
                            character = char.ToUpper(character);
                        }

                        output += character;
                    }
                }

                if (spaceCount > 0)
                {
                    output += spaceCount;
                    spaceCount = 0;
                }

                if (y != 0)
                {
                    output += '/';
                }
            }

            return output;
        }

        private string GetCastlingStateFen(CastlingState state)
        {
            if (!state.WhiteKingside && !state.WhiteQueenside && !state.BlackKingside && !state.BlackQueenside)
            {
                return "-";
            }

            string output = "";
            if (state.WhiteKingside)
            {
                output += 'K';
            }

            if (state.WhiteQueenside)
            {
                output += 'Q';
            }

            if (state.BlackKingside)
            {
                output += 'k';
            }

            if (state.BlackQueenside)
            {
                output += 'q';
            }

            return output;
        }
    }
}
