using System.Collections.Generic;
using System.Linq;

namespace Chess.Api.MoveValidation
{
    public enum PieceType
    {
        Pawn,
        King,
        Queen,
        Knight,
        Bishop,
        Rook
    }

    public static class PieceTypeExtensions
    {
        private static readonly Dictionary<PieceType, char> _pieceTypeToCharacter = new Dictionary<PieceType, char>
        {
            {PieceType.Pawn, 'p'},
            {PieceType.Knight, 'n'},
            {PieceType.Bishop, 'b'},
            {PieceType.Rook, 'r'},
            {PieceType.Queen, 'q'},
            {PieceType.King, 'k'}
        };

        private static readonly Dictionary<char, PieceType> _characterToPieceType;

        static PieceTypeExtensions()
        {
            _characterToPieceType = _pieceTypeToCharacter.ToDictionary(dict => dict.Value, dict => dict.Key); // Invert character to piece dictionary
        }

        public static char ToCharacter(this PieceType pieceType)
        {
            return _pieceTypeToCharacter[pieceType];
        }

        public static PieceType? ToPieceType(this char character)
        {
            character = char.ToLower(character);

            if (_characterToPieceType.ContainsKey(character))
            {
                return _characterToPieceType[character];
            }

            return null;
        }
    }
}
