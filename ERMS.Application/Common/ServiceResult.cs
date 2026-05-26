namespace ERMS.Application.Common
{
    public class ServiceResult<T>
    {
        public bool Success { get; private set; }
        public T? Data { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public int StatusCode { get; private set; }

        public static ServiceResult<T> Ok(T data, string message = "Success")
        {
            return new ServiceResult<T> { Success = true, Data = data, Message = message, StatusCode = 200 };
        }

        public static ServiceResult<T> Created(T data, string message = "Created successfully")
        {
            return new ServiceResult<T> { Success = true, Data = data, Message = message, StatusCode = 201 };
        }

        public static ServiceResult<T> Fail(string message, int statusCode = 400)
        {
            return new ServiceResult<T> { Success = false, Message = message, StatusCode = statusCode };
        }

        public static ServiceResult<T> NotFound(string message = "Resource not found")
        {
            return new ServiceResult<T> { Success = false, Message = message, StatusCode = 404 };
        }

        public static ServiceResult<T> Unauthorized(string message = "Unauthorized")
        {
            return new ServiceResult<T> { Success = false, Message = message, StatusCode = 401 };
        }

        public static ServiceResult<T> Forbidden(string message = "Forbidden")
        {
            return new ServiceResult<T> { Success = false, Message = message, StatusCode = 403 };
        }
    }
}
