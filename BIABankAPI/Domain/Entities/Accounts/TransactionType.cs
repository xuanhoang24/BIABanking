namespace BankingSystemAPI.Domain.Entities.Accounts
{
    public enum TransactionType
    {
        Transfer = 1,
        Deposit = 2,
        Withdrawal = 3,
        Fee = 4,
        Interest = 5,
        Refund = 6
    }
}
