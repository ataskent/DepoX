namespace DepoX.Services.Erp;

public sealed class ServiceResult<T>
{
    public bool Success { get; }
    public string? Message { get; }
    public string? ErrorCode { get; }
    public T? Data { get; }

    private ServiceResult(bool success, T? data, string? message, string? errorCode)
    {
        Success = success;
        Data = data;
        Message = message;
        ErrorCode = errorCode;
    }

    public static ServiceResult<T> Ok(T data, string? message = null)
        => new(true, data, message, null);

    public static ServiceResult<T> Fail(string message, string? errorCode = null)
        => new(false, default, message, errorCode);
}
