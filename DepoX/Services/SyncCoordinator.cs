using DepoX.Services;
using DepoX.Services.Erp;

public class SyncCoordinator
{
    private readonly LocalDataService _localData;
    private readonly IErpCommitGateway _commitGateway;

    public SyncCoordinator(
        LocalDataService localData,
        IErpCommitGateway commitGateway)
    {
        _localData = localData;
        _commitGateway = commitGateway;
    }

    public async Task<SyncResult> CommitAsync(
        CancellationToken cancellationToken = default)
    {
        var pendingTransactions =
            await _localData.GetPendingTransactionsAsync(cancellationToken);

        foreach (var tx in pendingTransactions)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result =
                await _commitGateway.CommitAsync(tx, cancellationToken);

            if (result.Success)
            {
                await _localData.MarkAsSentAsync(tx.Id, cancellationToken);
            }
            else
            {
                await _localData.MarkAsFailedAsync(
                    tx.Id,
                    result.Message ?? "Unknown ERP error",
                    cancellationToken);
            }
        }

        return SyncResult.Success();
    }
}
