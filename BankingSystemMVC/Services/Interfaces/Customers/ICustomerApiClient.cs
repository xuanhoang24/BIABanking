using BankingSystemMVC.Models.Common;
using BankingSystemMVC.Models.Kyc;
using BankingSystemMVC.Models.ViewModels.Customers;
using BankingSystemMVC.Models.ViewModels.Kyc;

namespace BankingSystemMVC.Services.Interfaces.Customers
{
    public interface ICustomerApiClient
    {
        Task<CustomerMeViewModel?> GetMeAsync();
        Task<(bool Success, string? Error)> UploadKycAsync(UploadKycViewModel model);
        Task<KycSubmissionViewModel?> GetMyKycAsync();
        Task<ApiFileResult?> GetMyKycFileAsync();
        Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);
    }
}
