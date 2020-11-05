using Chess.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Chess.Api.Constants;
using Chess.Api.Models;
using Chess.Api.Models.Database;
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

            if (!game.Active)
            {
                await Clients.Caller.SendAsync(GameHubOutgoingMessages.ILLEGAL_MOVE, game.Fen);
                return;
            }

            var moveValidationResult = _moveValidator.ValidateMove(game.Fen, moveNotation);
            if (!moveValidationResult.IsValid)
            {
                await Clients.Caller.SendAsync(GameHubOutgoingMessages.ILLEGAL_MOVE, game.Fen);
                return;
            }

            var move = _coordinateNotationParser.ParseNotationMove(moveNotation);
            var currentBoardState = _fenParser.ParseFen(game.Fen);
            var newBoardState = _moveHandler.ApplyMove(currentBoardState, move, moveValidationResult);

            _gameRepository.AddMoveToGame(gameId, moveNotation, newBoardState.Fen);
            await Clients.Group($"{GAME_GROUP_PREFIX}{gameId}")
                .SendAsync(GameHubOutgoingMessages.MOVE_PLAYED, moveNotation);

            if (moveValidationResult.ShouldResetHalfmoveClock)
            {
                _gameRepository.ClearPositionsFromIrreversibleMove(gameId);
            }

            _gameRepository.AddPositionToGame(gameId, newBoardState.Fen);
            var positionsSinceIrreversibleMove = _gameRepository.GetPositionsSinceIrreversibleMove(gameId);

            if (_moveValidator.IsCheckmate(newBoardState))
            {
                var winner = currentBoardState.ActiveColor;
                await EndGame(game, winner, GameConstants.CHECKMATE_TERMINATION, GameHubOutgoingMessages.CHECKMATE);
            }
            else if (_moveValidator.IsStalemate(newBoardState))
            {
                await EndGame(game, null, GameConstants.STALEMATE_TERMINATION, GameHubOutgoingMessages.DRAW);
            }
            else if (newBoardState.HalfmoveClock >= 100)
            {
                await EndGame(game, null, GameConstants.FIFTY_MOVE_RULE_TERMINATION, GameHubOutgoingMessages.DRAW);
            } else if (_moveValidator.IsThreefoldRepetition(newBoardState, positionsSinceIrreversibleMove))
            {
                await EndGame(game, null, GameConstants.THREEFOLD_REPETITION_TERMINATION, GameHubOutgoingMessages.DRAW);
            }
        }

        private async Task EndGame(GameDatabaseModel game, Color? winner, string termination, string signalRMessage)
        {
            if (winner != null)
            {
                _gameRepository.SetGameResult(game.Id, winner.Value == Color.White ? "white" : "black",
                    game.GetPlayerId(winner.Value), termination);
            }
            else
            {
                _gameRepository.SetGameResult(game.Id, null, null, termination);
            }

            var gameResult = new GameResult
            {
                WinnerColor = Color.White,
                Termination = termination
            };
            await Clients.Group($"{GAME_GROUP_PREFIX}{game.Id}").SendAsync(signalRMessage, gameResult);
        }
    }
}
