using BankingSystemAPI.Domain.Entities.Users.Customers;

namespace BankingSystemAPI.Application.Services.Interfaces.Admin
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
