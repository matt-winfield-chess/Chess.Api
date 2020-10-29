namespace Chess.Api.MoveValidation
{
    public class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }

        private readonly CoordinateNotationParser _coordinateNotationParser = new CoordinateNotationParser();

        public override string ToString()
        {
            return _coordinateNotationParser.ConvertCoordinateToString(this);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Move))
            {
                return false;
            }

            var coordinate = obj as Coordinate;
            return X == coordinate.X && Y == coordinate.Y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
