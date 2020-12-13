using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TwoPhaseCommitCoordinator.Repository;

namespace TwoPhaseCommitCoordinator
{
    public sealed class DecreaseStockQuantityTransaction : TwoPhaseTransaction
    {
        private readonly ILogger<DecreaseStockQuantityTransaction> _logger;
        private readonly IEnumerable<string> _requiredTransactionParams = new List<string> {"productId", "quantity"};
        private readonly StockRepository _stockRepository;

        public DecreaseStockQuantityTransaction(ITwoPhaseRepository twoPhaseRepository,
            Dictionary<string, string> transactionParams) : base(twoPhaseRepository,
            transactionParams)
        {
            if (transactionParams is null ||
                _requiredTransactionParams.Any(key => !transactionParams.ContainsKey(key)))
            {
                throw new MissingTransactionParamException();
            }

            _stockRepository = TwoPhaseRepository as StockRepository;
            _logger = ApplicationLogging.CreateLogger<DecreaseStockQuantityTransaction>();
            _logger.LogInformation($"DecreaseStockQuantityTransaction was prepared. Transaction Id = {TransactionId}");
        }


        public override async Task PrepareTransaction()
        {
            var productId = Convert.ToInt32(TransactionParams["productId"]);
            var quantity = Convert.ToInt32(TransactionParams["quantity"]);
            var stockQuantity = await _stockRepository.GetStockQuantity(productId);
            if (stockQuantity <= 0)
            {
                Status = TransactionStatus.Fail;
                _logger.LogWarning(
                    $"DecreaseStockQuantityTransaction status was changed as {Status} . Transaction Id = {TransactionId}");
            }
            else
            {
                try
                {
                    await _stockRepository.PrepareDecreaseStockQuantityTransaction(productId, quantity,
                        TransactionId);
                    Status = TransactionStatus.Prepared;
                    _logger.LogInformation(
                        $"DecreaseStockQuantityTransaction status was changed as {Status} . Transaction Id = {TransactionId}");
                }
                catch (Exception e)
                {
                    Status = TransactionStatus.Fail;
                    _logger.LogWarning(
                        $"DecreaseStockQuantityTransaction status was changed as {Status} . Transaction Id = {TransactionId}",e);
                }
            }
        }
    }
}