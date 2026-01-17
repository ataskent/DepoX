namespace DepoX.Services;

public class SyncResult
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }

    public static SyncResult Success()
        => new() { IsSuccess = true };

    public static SyncResult Failure(string error)
        => new() { IsSuccess = false, ErrorMessage = error };
}
