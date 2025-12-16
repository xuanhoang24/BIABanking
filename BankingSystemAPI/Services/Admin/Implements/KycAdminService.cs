using BankingSystemAPI.DataLayer;
using BankingSystemAPI.Models.Users.Customers;
using BankingSystemAPI.Services.Admin.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Services.Admin.Implements
{
    public class KycAdminService : IKycAdminService
    {
        private readonly AppDbContext _context;

        public KycAdminService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<KYCDocument?> GetPendingAsync(int kycId)
        {
            return await _context.KYCDocuments
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(x =>
                    x.Id == kycId &&
                    (x.Status == KYCStatus.Pending ||
                     x.Status == KYCStatus.UnderReview));
        }

        public async Task<List<KYCDocument>> GetPendingListAsync()
        {
            return await _context.KYCDocuments
                .Include(x => x.Customer)
                .Where(x =>
                    x.Status == KYCStatus.Pending ||
                    x.Status == KYCStatus.UnderReview)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task ApproveAsync(int kycId, int adminId)
        {
            var doc = await GetPendingAsync(kycId);
            if (doc == null)
                throw new InvalidOperationException("KYC not found or already processed");

            doc.Status = KYCStatus.Approved;
            doc.ReviewedByAdminId = adminId;
            doc.ReviewedAt = DateTime.UtcNow;

            doc.Customer!.IsKYCVerified = true;

            await _context.SaveChangesAsync();
        }

        public async Task RejectAsync(int kycId, int adminId, string reviewNotes)
        {
            var doc = await GetPendingAsync(kycId);
            if (doc == null)
                throw new InvalidOperationException("KYC not found or already processed");

            doc.Status = KYCStatus.Rejected;
            doc.ReviewNotes = reviewNotes;
            doc.ReviewedByAdminId = adminId;
            doc.ReviewedAt = DateTime.UtcNow;

            doc.Customer!.IsKYCVerified = false;

            await _context.SaveChangesAsync();
        }
    }
}
