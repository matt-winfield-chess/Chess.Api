using Microsoft.AspNetCore.Http;

namespace Chess.Api.Utils.Interfaces
{
    public interface IClaimsProvider
    {
        public int? GetId(HttpContext context);
    }
}
