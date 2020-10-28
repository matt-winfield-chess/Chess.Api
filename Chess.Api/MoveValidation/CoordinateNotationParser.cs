namespace Chess.Api.MoveValidation
{
    public class CoordinateNotationParser
    {
        private readonly int characterToNumberOffset = 'a';

        public Coordinate ParseNotationCoordinate(string notation)
        {
            var lowercaseNotation = notation.ToLower().Trim();

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
            var lowercaseNotation = notation.ToLower().Trim();

            if (lowercaseNotation.Length != 4) return null;

            return new Move
            {
                StartPosition = ParseNotationCoordinate(notation.Substring(0, 2)),
                EndPosition = ParseNotationCoordinate(notation.Substring(2))
            };
        }

        public string ConvertCoordinateToString(Coordinate coordinate)
        {
            return $"{(char) (coordinate.X) + characterToNumberOffset}{coordinate.Y + 1}";
        }
    }
}
