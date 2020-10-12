namespace Chess.Api.Responses
{
    public class ApiMethodResponse<T>
    {
        public T Data { get; set; }
        public string[] Errors { get; set; }
        public bool IsSuccess { get
            {
                if (Errors == null) return true;
                return Errors?.Length == 0;
            } 
        }
    }
}
