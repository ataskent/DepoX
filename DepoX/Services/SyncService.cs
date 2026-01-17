using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DepoX.Services
{
    public class SyncService
    {
        private readonly HttpClient _httpClient;
        private readonly LocalDataService _local;

        public SyncService(HttpClient httpClient, LocalDataService local)
        {
            _httpClient = httpClient;
            _local = local;
        }

        public async Task SyncAsync()
        {
            var pending = await _local.GetPendingTransactionsAsync();

            foreach (var tx in pending)
            {
                var response = await _httpClient.PostAsJsonAsync("https://api.myserver.com/transactions", tx);

                if (response.IsSuccessStatusCode)
                {
                    await _local.MarkAsSyncedAsync(tx.Id);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Sync failed: {error}");
                }
            }

            await _local.ClearSyncedAsync();
        }

    }
}
