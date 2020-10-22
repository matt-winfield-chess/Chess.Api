using System.Linq;
using Chess.Api.Models;
using Chess.Api.Repositories.Interfaces;
using Chess.Api.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Chess.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : Controller
    {
        private readonly IGameRepository _gameRepository;
        private readonly IUserRepository _userRepository;

        public GameController(IGameRepository gameRepository, IUserRepository userRepository)
        {
            _gameRepository = gameRepository;
            _userRepository = userRepository;
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
            var moves = movesDatabaseResponse.Select(move => new Move()
            {
                MoveNumber = move.MoveNumber,
                MoveString = move.Move
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
    }
}