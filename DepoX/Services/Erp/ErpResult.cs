public class ErpResult
{
    public bool Success { get; private set; }
    public string Message { get; private set; }
    public string ReferenceId { get; private set; }

    public static ErpResult Ok(string message, string referenceId = null)
    {
        return new ErpResult
        {
            Success = true,
            Message = message,
            ReferenceId = referenceId
        };
    }

    public static ErpResult Failed(string message)
    {
        return new ErpResult
        {
            Success = false,
            Message = message
        };
    }
}
