namespace BankingSystemMVC.Models.Kyc
{
    public enum DocumentType
    {
        Passport = 1,
        DriversLicense = 2,
        NationalId = 3,
        UtilityBill = 4,
        BankStatement = 5,
        TaxDocument = 6
    }

    public enum KYCStatus
    {
        Pending = 1,
        UnderReview = 2,
        Approved = 3,
        Rejected = 4,
        RequiresAdditionalInfo = 5
    }
}
