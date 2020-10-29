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

        public override bool Equals(object obj)
        {
            if (!(obj is Move))
            {
                return false;
            }

            var move = obj as Move;
            return StartPosition.Equals(move.StartPosition) && EndPosition.Equals(move.EndPosition);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
