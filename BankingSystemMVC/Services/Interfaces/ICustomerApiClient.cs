using BankingSystemMVC.Models.Customers;
using BankingSystemMVC.Models.Kyc;

namespace BankingSystemMVC.Services.Interfaces
{
    public interface ICustomerApiClient
    {
        Task<CustomerMeViewModel?> GetMeAsync();
        Task<bool> UploadKycAsync(UploadKycViewModel model);
        Task<List<KycSubmissionViewModel>> GetMyKycSubmissionsAsync();
    }
}
