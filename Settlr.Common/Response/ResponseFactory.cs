namespace Settlr.Common.Response;

public static class ResponseFactory
{
    public static Response<T> Success<T>(T data, string message, int statusCode)
    {
        return new Response<T>
        {
            Data = data,
            Succeeded = true,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static Response<T> Success<T>(string message, int statusCode)
    {
        return new Response<T>
        {
            Data = default,
            Succeeded = true,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static Response<T> Fail<T>(string message, int statusCode, string[]? errors = null)
    {
        return new Response<T>
        {
            Data = default,
            Succeeded = false,
            Message = message,
            Errors = errors,
            StatusCode = statusCode
        };
    }
}
