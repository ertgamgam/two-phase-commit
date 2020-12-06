using System.Data;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace TwoPhaseCommitCoordinator.Repository
{
    public class StockRepository : ITwoPhaseRepository
    {
        private const string ConnectionString =
            "User ID=postgres;Password=Gamgam123456;Host=localhost;Port=5432;Database=stock;Pooling=true;Timeout=5";

        private IDbConnection Connection
        {
            get
            {
                var npgsqlConnection = new NpgsqlConnection(ConnectionString);
                npgsqlConnection.Open();
                return npgsqlConnection;
            }
        }

        public async Task<bool> CommitTransaction(string transactionId)
        {
            using IDbConnection connection = Connection;
            return await connection.ExecuteAsync($"COMMIT PREPARED '{transactionId}'") > 0;
        }

        public async Task<bool> RollBackTransaction(string transactionId)
        {
            using var connection = Connection;
            return await connection.ExecuteAsync($"ROLLBACK PREPARED '{transactionId}'") > 0;
        }

        public async Task<int> GetStockQuantity(int productId)
        {
            using var connection = Connection;
            return await connection.ExecuteScalarAsync<int>(
                "SELECT quantity FROM dbo.stock WHERE  product_id=@productId",
                new {productId = productId});
        }

        public async Task<bool> PrepareDecreaseStockQuantityTransaction(int productId, int quantity,
            string transactionId)
        {
            using var connection = Connection;
            return await connection.ExecuteAsync($@"BEGIN;
                                                       UPDATE dbo.stock SET quantity=quantity - @quantity where product_id=@productId;
                                                       PREPARE TRANSACTION '{transactionId}';", new
                {quantity = quantity, productId = productId}) > 0;
        }
    }
}