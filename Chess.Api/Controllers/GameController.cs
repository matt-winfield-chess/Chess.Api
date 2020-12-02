using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chess.Api.Constants;
using Chess.Api.Models;
using Chess.Api.Models.Database;
using Chess.Api.Models.Patch;
using Chess.Api.MoveValidation;
using Chess.Api.Repositories.Interfaces;
using Chess.Api.Responses;
using Chess.Api.SignalR.Hubs;
using Chess.Api.SignalR.Messages;
using Chess.Api.Utils.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Chess.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : Controller
    {
        private readonly IGameRepository _gameRepository;
        private readonly IUserRepository _userRepository;
        private readonly IClaimsProvider _claimsProvider;
        private readonly IHubContext<GameHub> _gameHubContext;

        public GameController(IGameRepository gameRepository, IUserRepository userRepository,
            IClaimsProvider claimsProvider, IHubContext<GameHub> gameHubContext)
        {
            _gameRepository = gameRepository;
            _userRepository = userRepository;
            _claimsProvider = claimsProvider;
            _gameHubContext = gameHubContext;
        }

        [HttpGet("{gameId}")]
        public ActionResult<ApiMethodResponse<Game>> GetGame(string gameId)
        {
            var gameDatabaseResponse = _gameRepository.GetGameById(gameId);

            if (gameDatabaseResponse == null)
            {
                return NotFound(new ApiMethodResponse<Game>
                {
                    Errors = new[] {$"Game with ID {gameId} not found"}
                });
            }

            return Ok(new ApiMethodResponse<Game>
            {
                Data = MapGameDatabaseModelToGame(gameDatabaseResponse)
            });
        }

        [HttpGet("activeGames")]
        public ActionResult<ApiMethodResponse<IEnumerable<Game>>> GetActiveGames()
        {
            var id = _claimsProvider.GetId(HttpContext);

            if (id == null)
            {
                return Unauthorized(new ApiMethodResponse<IEnumerable<Game>>
                {
                    Errors = new [] { "No ID in token" }
                });
            }

            var gameDatabaseModels = _gameRepository.GetUserActiveGames(id.Value);

            var games = gameDatabaseModels.Select(MapGameDatabaseModelToGame);

            return Ok(new ApiMethodResponse<IEnumerable<Game>>
            {
                Data = games
            });
        }

        [HttpPatch("resign")]
        public async Task<ActionResult<ApiMethodResponse<bool>>> Resign(PatchResignationModel resignationModel)
        {
            var game = _gameRepository.GetGameById(resignationModel.GameId);
            var userId = _claimsProvider.GetId(HttpContext);

            if (game == null)
            {
                return NotFound(new ApiMethodResponse<bool>
                {
                    Data = false,
                    Errors = new[] {$"Game with id {resignationModel.GameId} not found"}
                });
            }

            if (!game.Active)
            {
                return BadRequest(new ApiMethodResponse<bool>
                {
                    Data = false,
                    Errors = new[] {$"Game {game.Id} is already complete"}
                });
            }

            if (userId == null || (userId != game.WhitePlayerId && userId != game.BlackPlayerId))
            {
                return Unauthorized(new ApiMethodResponse<bool>
                {
                    Data = false,
                    Errors = new[] {$"You are not a participant in game {game.Id}"}

                });
            }

            bool isWhitePlayer = userId == game.WhitePlayerId;

            _gameRepository.SetGameResult(game.Id,
                isWhitePlayer ? "black" : "white",
                isWhitePlayer ? game.BlackPlayerId : game.WhitePlayerId,
                GameConstants.RESIGNATION_TERMINATION);

            var gameResult = new GameResult
            {
                WinnerColor = isWhitePlayer ? Color.Black : Color.White,
                Termination = GameConstants.RESIGNATION_TERMINATION
            };
            await _gameHubContext.Clients.Group($"{GameHub.GAME_GROUP_PREFIX}{game.Id}").SendAsync(GameHubOutgoingMessages.RESIGNATION, gameResult);

            return Ok(new ApiMethodResponse<bool>
            {
                Data = true
            });
        }

        private Game MapGameDatabaseModelToGame(GameDatabaseModel game)
        {
            return new Game
            {
                Id = game.Id,
                WhitePlayer = _userRepository.GetUserById(game.WhitePlayerId),
                BlackPlayer = _userRepository.GetUserById(game.BlackPlayerId),
                Moves = _gameRepository.GetMovesByGameId(game.Id).Select(move => new MoveModel
                {
                    Move = move.Move,
                    MoveNumber = move.MoveNumber
                }),
                Active = game.Active,
                Fen = game.Fen,
                Winner = game.Winner,
                WinnerId = game.WinnerId,
                Termination = game.Termination
            };
        }
    }
}