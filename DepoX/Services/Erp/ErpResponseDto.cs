namespace DepoX.Services.Erp
{
    public class ErpResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public string ReferenceId { get; set; }
        public T Data { get; set; }
    }
}
