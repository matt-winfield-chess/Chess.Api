using Chess.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Chess.Api.Constants;
using Chess.Api.MoveValidation;
using Chess.Api.MoveValidation.Interfaces;
using Chess.Api.SignalR.Messages;

namespace Chess.Api.SignalR.Hubs
{
    public class GameHub : Hub
    {
        public const string GAME_GROUP_PREFIX = "Game-";

        private readonly IGameRepository _gameRepository;
        private readonly IMoveValidator _moveValidator;
        private readonly IMoveHandler _moveHandler;
        private readonly CoordinateNotationParser _coordinateNotationParser;
        private readonly FenParser _fenParser;

        public GameHub(IGameRepository gameRepository, IMoveValidator moveValidator, IMoveHandler moveHandler,
            CoordinateNotationParser coordinateNotationParser, FenParser fenParser)
        {
            _gameRepository = gameRepository;
            _moveValidator = moveValidator;
            _moveHandler = moveHandler;
            _coordinateNotationParser = coordinateNotationParser;
            _fenParser = fenParser;
        }

        public async Task JoinGame(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{GAME_GROUP_PREFIX}{gameId}");
        }

        public async Task LeaveGame(string gameId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{GAME_GROUP_PREFIX}{gameId}");
        }

        public async Task Move(string moveNotation, string gameId)
        {
            var game = _gameRepository.GetGameById(gameId);
            if (game == null)
            {
                return;
            }

            var moveValidationResult = _moveValidator.ValidateMove(game.Fen, moveNotation);
            if (!moveValidationResult.IsValid)
            {
                await Clients.Caller.SendAsync(GameHubOutgoingMessages.ILLEGAL_MOVE);
                return;
            }

            var move = _coordinateNotationParser.ParseNotationMove(moveNotation);
            var currentBoardState = _fenParser.ParseFen(game.Fen);
            var newBoardState = _moveHandler.ApplyMove(currentBoardState, move, moveValidationResult);

            _gameRepository.AddMoveToGame(gameId, moveNotation, newBoardState.Fen);
            await Clients.Group($"{GAME_GROUP_PREFIX}{gameId}").SendAsync(GameHubOutgoingMessages.MOVE_PLAYED, moveNotation);
        }
    }
}
