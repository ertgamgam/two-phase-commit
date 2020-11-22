using System;
using System.Threading.Tasks;
using TwoPhaseCommitCoordinator.Repository;

namespace TwoPhaseCommitCoordinator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var walletRepository = new WalletRepository();
            Console.WriteLine(await walletRepository.GetUserBalance(150));
            var transactionId = Guid.NewGuid().ToString();
            await walletRepository.PrepareDecreaseUserBalanceTransaction(150, 15, transactionId);
            Console.WriteLine(await walletRepository.GetUserBalance(150));
            await walletRepository.CommitTransaction(transactionId);
            Console.WriteLine(await walletRepository.GetUserBalance(150));
        }
    }
}