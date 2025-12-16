using BankingSystemAPI.Models.Users.Customers;

namespace BankingSystemAPI.Services.Admin.Interfaces
{
    public interface IKycAdminService
    {
        Task<KYCDocument?> GetPendingAsync(int kycId);
        Task<List<KYCDocument>> GetPendingListAsync();
        Task MarkUnderReviewAsync(int kycId, int adminId);
        Task ApproveAsync(int kycId, int adminId);
        Task RejectAsync(int kycId, int adminId, string reviewNotes);
    }
}
