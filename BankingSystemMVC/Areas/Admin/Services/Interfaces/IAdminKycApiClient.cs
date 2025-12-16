using BankingSystemMVC.Areas.Admin.Models;

namespace BankingSystemMVC.Areas.Admin.Services.Interfaces
{
    public interface IAdminKycApiClient
    {
        Task<List<AdminKycPendingViewModel>> GetPendingAsync();
        Task<AdminKycReviewViewModel?> GetForReviewAsync(int id); 
        Task<KycFileResult?> GetFileAsync(int id);
        Task MarkUnderReviewAsync(int id);
        Task ApproveAsync(int id);
        Task RejectAsync(int id, string reviewNotes);
    }
}
