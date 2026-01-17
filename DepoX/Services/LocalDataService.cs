using DepoX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace DepoX.Services
{
    public class LocalDataService
    {
        private readonly SQLiteAsyncConnection _db;

        public LocalDataService(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
            _db.CreateTableAsync<StockItem>().Wait();
            _db.CreateTableAsync<StockTransaction>().Wait();
        }

        public Task<int> SaveTransactionAsync(StockTransaction tx)
            => _db.InsertAsync(tx);

        public Task<List<StockTransaction>> GetPendingTransactionsAsync()
            => _db.Table<StockTransaction>().Where(t => !t.Synced).ToListAsync();

        public Task<int> MarkAsSyncedAsync(Guid id)
            => _db.ExecuteAsync("UPDATE StockTransaction SET Synced=1 WHERE Id=?", id);

        public Task<int> ClearSyncedAsync()
            => _db.ExecuteAsync("DELETE FROM StockTransaction WHERE Synced=1");

        public Task<List<StockItem>> GetStockAsync(string whouseCode)
            => _db.Table<StockItem>().Where(s => s.WhouseCode == whouseCode).ToListAsync();

        public Task<int> UpdateStockAsync(StockItem item)
            => _db.UpdateAsync(item);

        public Task<List<StockTransaction>> GetPendingTransactionsAsync(CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task MarkAsSentAsync(Guid transactionId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task MarkAsFailedAsync(Guid transactionId, string error, CancellationToken ct)
        {
            throw new NotImplementedException();
        }


    }
}
