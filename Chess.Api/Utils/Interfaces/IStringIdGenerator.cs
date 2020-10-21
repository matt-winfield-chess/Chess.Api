namespace Chess.Api.Utils.Interfaces
{
    public interface IStringIdGenerator
    {
        public string GenerateId(int length = 10);
    }
}
