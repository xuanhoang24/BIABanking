namespace BankingSystemAPI.Domain.Entities.Accounts
{
    public enum TransactionStatus
    {
        Pending = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4,
        Processing = 5
    }
}
