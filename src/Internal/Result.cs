using System;

namespace Internal
{
    internal class Result
    {
        private static readonly Result OkResult = new Result(false, null);

        protected Result(bool isFailure, string errorMessage)
        {
            IsFailure = isFailure;
            Error = errorMessage;
        }

        public bool IsSuccess => !IsFailure;
        public bool IsFailure { get; }
        public string Error { get; }

        public static Result Ok()
        {
            return OkResult;
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, false, null);
        }

        public static Result Fail(string error)
        {
            return new Result(true, error);
        }

        public static Result<T> Fail<T>(string error)
        {
            return new Result<T>(default, true, error);
        }
    }

    internal class Result<T> : Result
    {
        internal Result(T value, bool isFailure, string errorMessage) : base(isFailure, errorMessage)
        {
            Value = value;
        }

        public T Value { get; }

        public static implicit operator Result<T>(T value) => new Result<T>(value, false, null);
    }

    internal static class ResultExtensions
    {
        public static Result<T> OnFailure<T>(this Result<T> result, Action<string> action)
        {
            if (result.IsFailure)
                action(result.Error);

            return result;
        }

        public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action)
        {
            if (result.IsSuccess)
                action(result.Value);

            return result;
        }
    }
}
