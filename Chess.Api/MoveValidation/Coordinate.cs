namespace Chess.Api.MoveValidation
{
    public class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }

        private CoordinateNotationParser _coordinateNotationParser = new CoordinateNotationParser();

        public override string ToString()
        {
            return _coordinateNotationParser.ConvertCoordinateToString(this);
        }

        public override bool Equals(object value)
        {
            if (!(value is Move))
            {
                return false;
            }

            var coordinate = value as Coordinate;
            return X == coordinate.X && Y == coordinate.Y;
        }
    }
}
