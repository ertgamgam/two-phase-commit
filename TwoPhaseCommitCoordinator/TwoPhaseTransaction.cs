using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TwoPhaseCommitCoordinator.Repository;

namespace TwoPhaseCommitCoordinator
{
    public abstract class TwoPhaseTransaction
    {
        private readonly ILogger<TwoPhaseTransaction> _logger;

        protected TwoPhaseTransaction(ITwoPhaseRepository twoPhaseRepository,
            Dictionary<string, string> transactionParams)
        {
            TransactionParams = transactionParams;
            TwoPhaseRepository = twoPhaseRepository;
            _logger = ApplicationLogging.CreateLogger<TwoPhaseTransaction>();
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
            _logger.LogInformation(
                $"Transaction status was changed as {Status} . Transaction Id = {TransactionId}");
        }

        public async Task RollBackTransaction()
        {
            await TwoPhaseRepository.RollBackTransaction(TransactionId);
            Status = TransactionStatus.RollBacked;
            _logger.LogWarning(
                $"Transaction status was changed as {Status} . Transaction Id = {TransactionId}");
        }
    }
}