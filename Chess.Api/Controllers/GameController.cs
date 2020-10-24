using System.Collections.Generic;
using System.Linq;
using Chess.Api.Models;
using Chess.Api.Repositories.Interfaces;
using Chess.Api.Responses;
using Chess.Api.Utils.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Chess.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : Controller
    {
        private readonly IGameRepository _gameRepository;
        private readonly IUserRepository _userRepository;
        private readonly IClaimsProvider _claimsProvider;

        public GameController(IGameRepository gameRepository, IUserRepository userRepository, IClaimsProvider claimsProvider)
        {
            _gameRepository = gameRepository;
            _userRepository = userRepository;
            _claimsProvider = claimsProvider;
        }

        [HttpGet("{gameId}")]
        public ActionResult<ApiMethodResponse<Game>> GetGame(string gameId)
        {
            var gameDatabaseResponse = _gameRepository.GetGameById(gameId);
            var movesDatabaseResponse = _gameRepository.GetMovesByGameId(gameId);

            if (gameDatabaseResponse == null)
            {
                return NotFound(new ApiMethodResponse<Game>
                {
                    Errors = new[] {$"Game with ID {gameId} not found"}
                });
            }

            var whitePlayer = _userRepository.GetUserById(gameDatabaseResponse.WhitePlayerId);
            var blackPlayer = _userRepository.GetUserById(gameDatabaseResponse.BlackPlayerId);
            var moves = movesDatabaseResponse.Select(move => new MoveModel()
            {
                Move = move.Move,
                MoveNumber = move.MoveNumber
            });

            return Ok(new ApiMethodResponse<Game>
            {
                Data = new Game
                {
                    Id = gameId,
                    WhitePlayer = whitePlayer,
                    BlackPlayer = blackPlayer,
                    Moves = moves
                }
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

            var games = gameDatabaseModels.Select(game => new Game
            {
                Id = game.Id,
                WhitePlayer = _userRepository.GetUserById(game.WhitePlayerId),
                BlackPlayer = _userRepository.GetUserById(game.BlackPlayerId),
                Moves = _gameRepository.GetMovesByGameId(game.Id).Select(move => new MoveModel
                {
                    Move = move.Move,
                    MoveNumber = move.MoveNumber
                }),
                Active = game.Active
            });

            return Ok(new ApiMethodResponse<IEnumerable<Game>>
            {
                Data = games
            });
        }
    }
}