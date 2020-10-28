namespace Chess.Api.MoveValidation
{
    public class Move
    {
        public Coordinate StartPosition { get; set; }
        public Coordinate EndPosition { get; set; }
        public string MoveString
        {
            get => $"{StartPosition.ToString()}{EndPosition.ToString()}";
        }

        public override string ToString()
        {
            return MoveString;
        }

        public override bool Equals(object value)
        {
            if (!(value is Move))
            {
                return false;
            }

            var move = value as Move;
            return StartPosition.Equals(move.StartPosition) && EndPosition.Equals(move.EndPosition);
        }
    }
}
