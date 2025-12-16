using BankingSystemAPI.DataLayer;
using BankingSystemAPI.Models.Users.Admin;
using BankingSystemAPI.Models.Users.Customers;
using BankingSystemAPI.Services.Admin.Implements;
using BankingSystemAPI.Services.Kyc.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Services.Kyc.Implements
{
    public class KycService : IKycService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly AuditService _auditService;

        public KycService(AppDbContext context, IWebHostEnvironment env, AuditService auditService)
        {
            _context = context;
            _env = env;
            _auditService = auditService;
        }

        public async Task<KYCDocument> UploadAsync(int customerId, DocumentType documentType, IFormFile file)
        {
            var existing = await _context.KYCDocuments
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);

            if (existing != null)
            {
                if (existing.Status == KYCStatus.Pending ||
                    existing.Status == KYCStatus.UnderReview)
                {
                    throw new InvalidOperationException(
                        "KYC is currently under review. You cannot upload a new document."
                    );
                }

                _context.KYCDocuments.Remove(existing);
                await _context.SaveChangesAsync();
            }

            if (file.Length > 5 * 1024 * 1024)
                throw new InvalidOperationException("File too large");

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var doc = new KYCDocument
            {
                CustomerId = customerId,
                DocumentType = documentType,
                FileData = ms.ToArray(),
                ContentType = file.ContentType,
                OriginalFileName = file.FileName,
                Status = KYCStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.KYCDocuments.Add(doc);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                AuditAction.AccountCreated,
                "KYCDocument",
                doc.Id,
                customerId,
                $"Customer submitted KYC document: {documentType}"
            );

            return doc;
        }

        public async Task<KYCDocument?> GetCurrentCustomerDocumentAsync(int customerId)
        {
            return await _context.KYCDocuments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }
    }
}
