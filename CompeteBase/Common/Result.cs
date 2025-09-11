namespace Compete.Common
{
    public class Result
    {
        public int ErrorNo { get; set; } = 0;

        public string? Message { get; set; }
    }

    public class Result<T> : Result
    {
        public T? Data { get; set; }

        public Result() { }

        public Result(T data) => Data = data;

        public Result(int errorNo, string? message = null, T? data = default)
        {
            ErrorNo = errorNo;
            Message = message;
            Data = data;
        }
    }
}
