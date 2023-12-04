using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SampleWebApi.Common
{
    public class Result<T>
    {
        public Result(
            bool isSuccess,
            Error error,
            string successMessage = "",
            T data = default
            ) 
        {
             if ( isSuccess && error != Error.None || 
                !isSuccess && error == Error.None) 
             {
                throw new ArgumentException("Invalid Error", nameof(error));
             }
             IsSuccess = isSuccess;
            Error = error;
            Data = data;
            SuccessMessage = successMessage;
        }


        public bool IsSuccess { get; set; }
        public bool IsFailure => !IsSuccess;
        public T Data { get; set; }
        
        public Error Error { get; set; }
        public string SuccessMessage { get; set; }

        public static Result<T> Success(T data, string successMessage) => new(true, Error.None, successMessage, data);
        public static Result<T> Failure(Error error) => new(false, error);
    }

    public sealed record Error(string Code, string Description)
    {
        public static readonly Error None = new(string.Empty, string.Empty);
    }

}
