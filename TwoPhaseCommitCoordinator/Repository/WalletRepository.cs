using System.Data;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace TwoPhaseCommitCoordinator.Repository
{
    public class WalletRepository : ITwoPhaseRepository
    {
        private const string ConnectionString =
            "User ID=postgres;Password=Gamgam123456;Host=localhost;Port=5432;Database=wallet;Pooling=true;Timeout=1024";

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
            return await connection.ExecuteAsync($"COMMIT PREPARED '{transactionId}'", new
            {
                transactionId = transactionId
            }) > 0;
        }

        public async Task<bool> RollBackTransaction(string transactionId)
        {
            using var connection = Connection;
            return await connection.ExecuteAsync($"ROLLBACK PREPARED '{transactionId}'", new
            {
                transactionId = transactionId
            }) > 0;
        }

        public async Task<int> GetUserBalance(int userId)
        {
            using var connection = Connection;
            return await connection.ExecuteScalarAsync<int>("SELECT balance FROM dbo.balance WHERE user_id=@userId",
                new
                {
                    userId = userId
                });
        }

        public async Task<bool> PrepareDecreaseUserBalanceTransaction(int userId, int amount, string transactionId)
        {
            using var connection = Connection;
            return await connection.ExecuteAsync($@"BEGIN;
                                                       UPDATE dbo.balance SET balance=balance - @amount where user_id=@userId;
                                                       PREPARE TRANSACTION '{transactionId}';", new
            {
                amount = amount,
                userId = userId,
            }) > 0;
        }
    }
}