namespace LitShare.BLL.Common
{
    public class Result<T>
    {
        private Result(T value)
        {
            this.Value = value;
            this.IsSuccess = true;
            this.Error = string.Empty;
        }

        private Result(string error, bool isUnauthorized = false)
        {
            this.Value = default;
            this.IsSuccess = false;
            this.Error = error;
            this.IsUnauthorized = isUnauthorized;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !this.IsSuccess;

        public T? Value { get; }

        public string Error { get; }

        public bool IsUnauthorized { get; }

        public static Result<T> Success(T value) => new Result<T>(value);

        public static Result<T> Failure(string error) => new Result<T>(error);

        public static Result<T> Unauthorized(string error) => new Result<T>(error, true);
    }
}