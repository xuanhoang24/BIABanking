namespace BankingSystemAPI.Domain.Entities.Users.Admin
{
    public enum AuditAction
    {
        CustomerRegistration = 1,
        CustomerLogin = 2,
        CustomerLogout = 3,
        AccountCreated = 4,
        AccountFrozen = 5,
        AccountUnfrozen = 6,
        PasswordChanged = 7,
        ProfileUpdated = 8,
        AdminLogin = 9,
        AdminLogout = 10,
        AdminActionPerformed = 11,
        SuspiciousActivity = 12,
        TransactionInitiated = 13,
        TransactionCompleted = 14,
        TransactionFailed = 15,
        KYCSubmitted = 16,
        KYCApproved = 17,
        KYCRejected = 18,
        KYCDocumentUploaded = 19,
        ReportCreated = 20,
        ReportStatusUpdated = 21
    }
}
