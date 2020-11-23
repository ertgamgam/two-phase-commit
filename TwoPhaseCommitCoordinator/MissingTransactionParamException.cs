using System;

namespace TwoPhaseCommitCoordinator
{
    public class MissingTransactionParamException : Exception
    {
        public MissingTransactionParamException() : base("Some transaction params are missing")
        {
        }
    }
}