using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwoPhaseCommitCoordinator.Repository;

namespace TwoPhaseCommitCoordinator
{
    public class DecreaseWalletBalanceTransaction : TwoPhaseTransaction
    {
        private readonly IEnumerable<string> _requiredTransactionParams = new List<string> {"userId", "balance"};
        private readonly WalletRepository _walletRepository;

        public DecreaseWalletBalanceTransaction(ITwoPhaseRepository twoPhaseRepository,
            Dictionary<string, string> transactionParams) : base(twoPhaseRepository, transactionParams)
        {
            if (transactionParams is null ||
                _requiredTransactionParams.Any(key => !transactionParams.ContainsKey(key)))
            {
                throw new MissingTransactionParamException();
            }

            _walletRepository = TwoPhaseRepository as WalletRepository;
        }

        public override async Task PrepareTransaction()
        {
            var userId = Convert.ToInt32(TransactionParams["userId"]);
            var amount = Convert.ToInt32(TransactionParams["balance"]);
            var balance = await _walletRepository.GetUserBalance(userId);
            if (balance <= 0)
            {
                Status = TransactionStatus.Fail;
            }
            else
            {
                try
                {
                    await _walletRepository.PrepareDecreaseUserBalanceTransaction(
                        userId, amount,
                        TransactionId);
                    Status = TransactionStatus.Prepared;
                }
                catch (Exception e)
                {
                    Status = TransactionStatus.Fail;
                }
            }
        }
    }
}