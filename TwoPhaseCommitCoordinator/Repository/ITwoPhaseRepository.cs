using System.Threading.Tasks;

namespace TwoPhaseCommitCoordinator.Repository
{
    public interface ITwoPhaseRepository
    {
        Task<bool> CommitTransaction(string transactionId);
        Task<bool> RollBackTransaction(string transactionId);
    }
}