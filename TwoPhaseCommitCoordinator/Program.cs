using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwoPhaseCommitCoordinator.Repository;

namespace TwoPhaseCommitCoordinator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var stockRepository = new StockRepository();
            var walletRepository = new WalletRepository();


            var twoPhaseTransactions = new List<TwoPhaseTransaction>()
            {
                new DecreaseStockQuantityTransaction(stockRepository,
                    new Dictionary<string, string>() {{"productId", "12"}, {"quantity", "10"}}),
                new DecreaseWalletBalanceTransaction(walletRepository,
                    new Dictionary<string, string>() {{"userId", "150"}, {"totalPrice", "4000"}})
            };

            var tasks = new List<Task>();
            twoPhaseTransactions.ForEach(x => tasks.Add(x.PrepareTransaction()));
            Task.WaitAll(tasks.ToArray());
            tasks.Clear();

            if (twoPhaseTransactions.All(x => x.Status is TransactionStatus.Prepared))
            {
                twoPhaseTransactions.ForEach(x => tasks.Add(x.CommitTransaction()));
            }

            else
            {
                twoPhaseTransactions.Where(x => x.Status is TransactionStatus.Prepared)
                    .ToList().ForEach(x => tasks.Add(x.RollBackTransaction()));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}