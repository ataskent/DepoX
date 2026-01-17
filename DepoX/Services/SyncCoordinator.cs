using DepoX.Services.Erp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DepoX.Services;

public class SyncCoordinator
{
    private readonly LocalDataService _localData;
    private readonly IErpGateway _erpGateway;

    public SyncCoordinator(
        LocalDataService localData,
        IErpGateway erpGateway)
    {
        _localData = localData;
        _erpGateway = erpGateway;
    }

    public async Task<SyncResult> CommitAsync(
        CancellationToken cancellationToken = default)
    {
        var pendingTransactions =
            await _localData.GetPendingTransactionsAsync(cancellationToken);

        foreach (var tx in pendingTransactions)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _erpGateway.CommitAsync(tx, cancellationToken);

            if (result.Success)
            {
                await _localData.MarkAsSentAsync(tx.Id, cancellationToken);
            }
            else
            {
                await _localData.MarkAsFailedAsync(
                    tx.Id,
                    result.ErrorMessage ?? "Unknown ERP error",
                    cancellationToken);
            }
        }

        return SyncResult.Success();
    }
}
