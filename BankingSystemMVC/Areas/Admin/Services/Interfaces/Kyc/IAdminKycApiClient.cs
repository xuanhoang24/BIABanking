using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Models.ViewModels.Kyc;

namespace BankingSystemMVC.Areas.Admin.Services.Interfaces.Kyc
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
