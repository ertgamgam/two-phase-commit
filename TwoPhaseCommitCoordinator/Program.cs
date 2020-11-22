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
            var userBalance = await walletRepository.GetUserBalance(123);
            Console.WriteLine(userBalance);
        }
    }
}