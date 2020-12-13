using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TwoPhaseCommitCoordinator.Repository;

namespace TwoPhaseCommitCoordinator
{
    public class DecreaseWalletBalanceTransaction : TwoPhaseTransaction
    {
        private readonly ILogger<DecreaseStockQuantityTransaction> _logger;
        private readonly IEnumerable<string> _requiredTransactionParams = new List<string> {"userId", "totalPrice"};
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
            _logger = ApplicationLogging.CreateLogger<DecreaseStockQuantityTransaction>();
            _logger.LogInformation($"DecreaseWalletBalanceTransaction was prepared. Transaction Id = {TransactionId}");
        }

        public override async Task PrepareTransaction()
        {
            var userId = Convert.ToInt32(TransactionParams["userId"]);
            var totalPrice = Convert.ToInt32(TransactionParams["totalPrice"]);
            var balance = await _walletRepository.GetUserBalance(userId);
            if (balance <= 0)
            {
                Status = TransactionStatus.Fail;
                _logger.LogWarning(
                    $"DecreaseWalletBalanceTransaction status was changed as {Status} . Transaction Id = {TransactionId}");
            }
            else
            {
                try
                {
                    await _walletRepository.PrepareDecreaseUserBalanceTransaction(
                        userId, totalPrice,
                        TransactionId);
                    Status = TransactionStatus.Prepared;
                    _logger.LogInformation(
                        $"DecreaseWalletBalanceTransaction status was changed as {Status} . Transaction Id = {TransactionId}");
                }
                catch (Exception e)
                {
                    Status = TransactionStatus.Fail;
                    _logger.LogWarning(
                        $"DecreaseWalletBalanceTransaction status was changed as {Status} . Transaction Id = {TransactionId}",
                        e);
                }
            }
        }
    }
}