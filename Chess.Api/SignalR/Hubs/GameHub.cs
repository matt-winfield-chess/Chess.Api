using Chess.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Chess.Api.SignalR.Messages;

namespace Chess.Api.SignalR.Hubs
{
    public class GameHub : Hub
    {
        public const string GAME_GROUP_PREFIX = "Game-";

        private IGameRepository _gameRepository;

        public GameHub(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task JoinGame(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{GAME_GROUP_PREFIX}{gameId}");
        }

        public async Task LeaveGame(string gameId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{GAME_GROUP_PREFIX}{gameId}");
        }

        public async Task Move(string move, string gameId)
        {
            _gameRepository.AddMoveToGame(gameId, move);
            await Clients.Group($"{GAME_GROUP_PREFIX}{gameId}").SendAsync(GameHubOutgoingMessages.MOVE_PLAYED, move);
        }
    }
}
