namespace DepoX.Services.Erp
{
    public class ErpResult
    {
        public bool Success { get; }
        public string? ErrorMessage { get; }

        private ErpResult(bool success, string? errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public static ErpResult Ok() => new ErpResult(true, null);

        public static ErpResult Failed(string errorMessage) => new ErpResult(false, errorMessage);
    }
}
