namespace Chess.Api.Utils.Interfaces
{
    public interface IStringIdGenerator
    {
        public string GenerateId();
        public string GenerateId(int length);
    }
}
