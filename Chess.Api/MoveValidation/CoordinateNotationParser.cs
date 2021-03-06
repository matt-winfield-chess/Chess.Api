﻿using System.Globalization;

namespace Chess.Api.MoveValidation
{
    public class CoordinateNotationParser
    {
        private readonly int characterToNumberOffset = 'a';

        public Coordinate ParseNotationCoordinate(string notation)
        {
            var lowercaseNotation = notation.ToLower(CultureInfo.InvariantCulture).Trim();

            if (lowercaseNotation.Length != 2) return null;

            int x = lowercaseNotation[0] - characterToNumberOffset;
            int y = (int) char.GetNumericValue(lowercaseNotation[1]) - 1;

            return new Coordinate
            {
                X = x,
                Y = y
            };
        }

        public Move ParseNotationMove(string notation)
        {
            var lowercaseNotation = notation.ToLower(CultureInfo.InvariantCulture).Trim();

            if (lowercaseNotation.Length < 4 || lowercaseNotation.Length > 5) return null;

            return new Move
            {
                StartPosition = ParseNotationCoordinate(notation.Substring(0, 2)),
                EndPosition = ParseNotationCoordinate(notation.Substring(2, 2)),
                Promotion = lowercaseNotation.Length != 5
                    ? null
                    : lowercaseNotation[4].ToPieceType()
            };
        }

        public string ConvertCoordinateToString(Coordinate coordinate)
        {
            char x = (char) (characterToNumberOffset + coordinate.X);
            return $"{x}{coordinate.Y + 1}";
        }
    }
}
