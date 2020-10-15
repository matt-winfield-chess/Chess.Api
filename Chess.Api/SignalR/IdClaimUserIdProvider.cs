using Microsoft.AspNetCore.SignalR;
using System.Linq;

namespace Chess.Api.SignalR
{
    public class IdClaimUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User.Claims.First(claim => claim.Type == "id").Value;
        }
    }
}
