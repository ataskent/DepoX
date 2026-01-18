namespace DepoX.Dtos
{
    public class ErpResult<T>
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }
        public string ErrorCode { get; private set; }
        public string ReferenceId { get; private set; }
        public T Data { get; private set; }

        public static ErpResult<T> Ok(T data, string message, string referenceId)
        {
            return new ErpResult<T>
            {
                Success = true,
                Data = data,
                Message = message,
                ReferenceId = referenceId
            };
        }

        public static ErpResult<T> Failed(string errorCode, string message)
        {
            return new ErpResult<T>
            {
                Success = false,
                ErrorCode = errorCode,
                Message = message
            };
        }
    }
}
