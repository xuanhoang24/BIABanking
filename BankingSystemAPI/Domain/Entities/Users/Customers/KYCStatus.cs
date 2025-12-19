namespace BankingSystemAPI.Domain.Entities.Users.Customers
{
    public enum KYCStatus
    {
        Pending = 1,
        UnderReview = 2,
        Approved = 3,
        Rejected = 4,
        RequiresAdditionalInfo = 5
    }
}
