using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalExtensionsLibrary.Exceptions
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; private set; }
        public string StackTrace { get; private set; }

        protected Result(bool isSuccess, string error)
        {
            if (isSuccess && !string.IsNullOrWhiteSpace(error))
                throw new InvalidOperationException();
            if (!isSuccess && string.IsNullOrWhiteSpace(error))
                throw new InvalidOperationException();

            IsSuccess = isSuccess;
            Error = error;

        }

        protected Result(bool isSuccess, string errorMessage, string stackTrace)
        {
            if (isSuccess && !string.IsNullOrWhiteSpace(errorMessage))
                throw new InvalidOperationException();
            if (!isSuccess && string.IsNullOrWhiteSpace(errorMessage))
                throw new InvalidOperationException();

            IsSuccess = isSuccess;
            Error = errorMessage;
            StackTrace = stackTrace;
        }

        public static Result Fail(string message)
        {
            return new Result(false, message);
        }

        public static Result Fail(string errorMessage, string stackTrace)
        {
            return new Result(false, errorMessage, stackTrace);
        }

        public static Result<T> Fail<T>(string message)
        {
            return new Result<T>(default(T), false, message);
        }

        public static Result<T> Fail<T>(string errorMessage, string stackTrace)
        {
            return new Result<T>(default(T), false, errorMessage, stackTrace);
        }

        public static Result Ok()
        {
            return new Result(true, string.Empty);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, true, string.Empty);
        }
        

        public static Result Combine(params Result[] results)
        {
            foreach(Result result in results)
            {
                if (result.IsFailure)
                    return result;
            }

            return Ok();
        }
    }


    public class Result<T> : Result
    {
        private readonly T _value;

        public T Value
        {
            get
            {
                if (!IsSuccess)
                    throw new InvalidOperationException();
                return _value;
            }
        }

        protected internal Result(T value, bool isSuccess, string error) : base(isSuccess, error)
        {
            _value = value;
        }

        protected internal Result(T value, bool isSuccess, string errorMessage, string stackTrace)
            : base(isSuccess, errorMessage, stackTrace)
        {
            _value = value;
        }
    }

}
