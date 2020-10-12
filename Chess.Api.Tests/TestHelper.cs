using Microsoft.AspNetCore.Mvc;

namespace Chess.Api.Tests
{
    public static class TestHelper
    {
        public static T ConvertObjectResponse<T, TObjectResult>(ActionResult<T> actionResult) where TObjectResult : ObjectResult
        {
            return (T) ((TObjectResult) actionResult.Result).Value;
        }
    }
}
