using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwoPhaseCommitCoordinator.Repository;

namespace TwoPhaseCommitCoordinator
{
    public abstract class TwoPhaseTransaction
    {
        protected TwoPhaseTransaction(ITwoPhaseRepository twoPhaseRepository,
            Dictionary<string, string> transactionParams)
        {
            TransactionParams = transactionParams;
            TwoPhaseRepository = twoPhaseRepository;
        }

        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
        protected ITwoPhaseRepository TwoPhaseRepository { get; }
        protected Dictionary<string, string> TransactionParams { get; set; }

        protected string TransactionId { get; } = Guid.NewGuid().ToString();
        public abstract Task PrepareTransaction();

        public async Task CommitTransaction()
        {
            await TwoPhaseRepository.CommitTransaction(TransactionId);
            Status = TransactionStatus.Commited;
        }

        public async Task RollBackTransaction()
        {
            await TwoPhaseRepository.RollBackTransaction(TransactionId);
            Status = TransactionStatus.RollBacked;
        }
    }
}