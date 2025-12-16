using BankingSystemAPI.DataLayer;
using BankingSystemAPI.Models.Users.Customers;
using BankingSystemAPI.Services.Kyc.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Services.Kyc.Implements
{
    public class KycService : IKycService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public KycService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<KYCDocument> UploadAsync(int customerId, DocumentType documentType, IFormFile file)
        {
            var uploadsRoot = Path.Combine(
                _env.ContentRootPath,
                "Uploads",
                "KYC",
                customerId.ToString()
            );

            Directory.CreateDirectory(uploadsRoot);

            var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var storedFilePath = Path.Combine(uploadsRoot, storedFileName);

            using (var stream = new FileStream(storedFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var kycDocument = new KYCDocument
            {
                CustomerId = customerId,
                DocumentType = documentType,
                FilePath = storedFilePath,
                OriginalFileName = file.FileName,
                Status = KYCStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.KYCDocuments.Add(kycDocument);
            await _context.SaveChangesAsync();

            return kycDocument;
        }

        public async Task<List<KYCDocument>> GetMyDocumentsAsync(int customerId)
        {
            return await _context.KYCDocuments
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}
