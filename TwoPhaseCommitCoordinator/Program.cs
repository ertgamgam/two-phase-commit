using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwoPhaseCommitCoordinator.Repository;

namespace TwoPhaseCommitCoordinator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // var walletRepository = new WalletRepository();
            // Console.WriteLine(await walletRepository.GetUserBalance(150));
            // var transactionId = Guid.NewGuid().ToString();
            // await walletRepository.PrepareDecreaseUserBalanceTransaction(150, 15, transactionId);
            // Console.WriteLine(await walletRepository.GetUserBalance(150));
            // await walletRepository.CommitTransaction(transactionId);
            // Console.WriteLine(await walletRepository.GetUserBalance(150));
            //
            // Console.WriteLine("-----");
            // var stockRepository = new StockRepository();
            // Console.WriteLine(await stockRepository.GetStockQuantity(12));
            // await stockRepository.PrepareDecreaseStockQuantityTransactionTransaction(12, 10, transactionId);
            // Console.WriteLine(await stockRepository.GetStockQuantity(12));
            // await stockRepository.CommitTransaction(transactionId);
            // Console.WriteLine(await stockRepository.GetStockQuantity(12));

            Console.WriteLine("-----");
            var stockRepository = new StockRepository();
            TwoPhaseTransaction decreaseStockQuantityTransaction = new DecreaseStockQuantityTransaction(stockRepository,
                new Dictionary<string, string>()
                    {{"productId", "12"}, {"quantity", "10"}});

            Console.WriteLine(decreaseStockQuantityTransaction.Status);
            await decreaseStockQuantityTransaction.PrepareTransaction();
            if (decreaseStockQuantityTransaction.Status is TransactionStatus.Prepared
            ) //TODO : SHOULD MOVE TO BASE CLASS
            {
                await decreaseStockQuantityTransaction.CommitTransaction();
            }

            Console.ReadLine();
        }
    }
}