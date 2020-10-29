using System.Collections.Generic;
using Chess.Api.MoveValidation.MovementStrategies;

namespace Chess.Api.MoveValidation
{
    public class MovementStrategyProvider
    {
        private readonly Dictionary<PieceType, IEnumerable<IMovementStrategy>> _pieceTypeDictionary =
            new Dictionary<PieceType, IEnumerable<IMovementStrategy>>
            {
                {PieceType.Pawn, new[] {new PawnMovementStrategy()}},
                {PieceType.Knight, new[] {new KnightMovementStrategy()}},
                {PieceType.Bishop, new[] {new DiagonalMovementStrategy()}},
                {PieceType.Rook, new[] {new StraightMovementStrategy()}},
                {PieceType.Queen, new IMovementStrategy[] {new DiagonalMovementStrategy(), new StraightMovementStrategy()}},
                {PieceType.King, new IMovementStrategy[] {new SingleSquareMovementStrategy(), new CastleMovementStrategy()}}
            };

        public IEnumerable<IMovementStrategy> GetStrategies(PieceType pieceType)
        {
            return _pieceTypeDictionary[pieceType];
        }
    }
}
