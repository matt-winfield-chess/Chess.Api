using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.Api.MoveValidation
{
    public class CoordinateNotationParser
    {
        private readonly int characterToNumberOffset = 'a';

        public Coordinate ParseNotationCoordinate(string notation)
        {
            var lowercaseNotation = notation.ToLower();

            if (lowercaseNotation.Length != 2) return null;

            int x = lowercaseNotation[0] - characterToNumberOffset;
            int y = 8 - (int) char.GetNumericValue(lowercaseNotation[1]);

            return new Coordinate
            {
                X = x,
                Y = y
            };
        }
    }
}
