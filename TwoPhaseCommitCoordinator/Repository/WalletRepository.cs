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

        private IDbConnection Connection => new NpgsqlConnection(ConnectionString);

        public async Task<bool> CommitTransaction(string transactionId)
        {
            using (IDbConnection connection = Connection)
            {
                connection.Open();
                ;
                return await connection.ExecuteAsync("COMMIT PREPARED @transactionId", new
                {
                    transactionId = transactionId
                }) > 0;
            }
        }

        public async Task<bool> RoleBackTransaction(string transactionId)
        {
            using (var connection = Connection)
            {
                connection.Open();
                return await connection.ExecuteAsync("ROLLBACK PREPARED @transactionId", new
                {
                    transactionId = transactionId
                }) > 0;
            }
        }

        public async Task<int> GetUserBalance(int userId)
        {
            using (var connection = Connection)
            {
                connection.Open();
                return await connection.ExecuteScalarAsync<int>("SELECT balance FROM dbo.balance WHERE user_id=123");
            }
        }
    }
}