namespace TwoPhaseCommitCoordinator
{
    public enum TransactionStatus
    {
        Pending,
        Prepared,
        Fail,
        Commited,
        RollBacked
    }
}