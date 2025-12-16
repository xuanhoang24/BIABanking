using BankingSystemAPI.Models.Users.Customers;

namespace BankingSystemAPI.Services.Kyc.Interfaces
{
    public interface IKycService
    {
        Task<KYCDocument> UploadAsync(int customerId, DocumentType documentType, IFormFile file);

        Task<KYCDocument?> GetCurrentCustomerDocumentAsync(int customerId);
    }
}
