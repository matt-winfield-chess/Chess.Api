using System.Linq;
using Chess.Api.Utils.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Chess.Api.Utils
{
    public class ClaimsProvider : IClaimsProvider
    {
        public int? GetId(HttpContext context)
        {
            var claims = context.User.Claims;
            try
            {
                return int.Parse(claims.FirstOrDefault(claim => claim.Type == "id")?.Value);
            }
            catch
            {
                return null;
            }
            
        }
    }
}
