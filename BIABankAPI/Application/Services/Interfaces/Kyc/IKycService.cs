using BankingSystemAPI.Domain.Entities.Users.Customers;

namespace BankingSystemAPI.Application.Services.Interfaces.Kyc
{
    public interface IKycService
    {
        Task<KYCDocument> UploadAsync(int customerId, DocumentType documentType, IFormFile file);

        Task<KYCDocument?> GetCurrentCustomerDocumentAsync(int customerId);
    }
}
