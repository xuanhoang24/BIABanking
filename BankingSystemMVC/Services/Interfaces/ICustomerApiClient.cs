using BankingSystemMVC.Models.Common;
using BankingSystemMVC.Models.Customers;
using BankingSystemMVC.Models.Kyc;

namespace BankingSystemMVC.Services.Interfaces
{
    public interface ICustomerApiClient
    {
        Task<CustomerMeViewModel?> GetMeAsync();
        Task<(bool Success, string? Error)> UploadKycAsync(UploadKycViewModel model);
        Task<KycSubmissionViewModel?> GetMyKycAsync();
        Task<ApiFileResult?> GetMyKycFileAsync();
    }
}
