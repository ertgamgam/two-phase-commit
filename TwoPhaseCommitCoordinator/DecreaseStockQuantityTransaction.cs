using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwoPhaseCommitCoordinator.Repository;

namespace TwoPhaseCommitCoordinator
{
    public sealed class DecreaseStockQuantityTransaction : TwoPhaseTransaction
    {
        private IEnumerable<string> RequiredTransactionParams => new List<string> {"productId", "quantity"};
        private readonly StockRepository _stockRepository;

        public DecreaseStockQuantityTransaction(ITwoPhaseRepository twoPhaseRepository,
            Dictionary<string, string> transactionParams) : base(twoPhaseRepository,
            transactionParams)
        {
            if (transactionParams is null ||
                RequiredTransactionParams.Any(key => !transactionParams.ContainsKey(key)))
            {
                throw new MissingTransactionParamException();
            }

            _stockRepository = TwoPhaseRepository as StockRepository;
        }


        public override async Task PrepareTransaction()
        {
            var productId = Convert.ToInt32(TransactionParams["productId"]);
            var quantity = Convert.ToInt32(TransactionParams["quantity"]);
            var stockQuantity = await _stockRepository.GetStockQuantity(productId);
            if (stockQuantity <= 0)
            {
                Status = TransactionStatus.Fail;
            }
            else
            {
                await _stockRepository.PrepareDecreaseStockQuantityTransactionTransaction(productId, quantity,
                    TransactionId);
                Status = TransactionStatus.Prepared;
            }
        }
    }
}