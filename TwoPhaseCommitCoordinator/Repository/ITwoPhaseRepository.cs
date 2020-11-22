using System.Threading.Tasks;

namespace TwoPhaseCommitCoordinator.Repository
{
    interface ITwoPhaseRepository
    {
        Task<bool> CommitTransaction(string transactionId);
        Task<bool> RoleBackTransaction(string transactionId);
    }
}