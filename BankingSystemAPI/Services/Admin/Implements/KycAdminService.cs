using BankingSystemAPI.DataLayer;
using BankingSystemAPI.Models.Users.Admin;
using BankingSystemAPI.Models.Users.Customers;
using BankingSystemAPI.Services.Admin.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Services.Admin.Implements
{
    public class KycAdminService : IKycAdminService
    {
        private readonly AppDbContext _context;
        private readonly AuditService _auditService;

        public KycAdminService(AppDbContext context, AuditService auditService)
        {
            _context = context;
            _auditService = auditService;
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
                .Include(x => x.ReviewedByAdmin)
                .Where(x =>
                    x.Status == KYCStatus.Pending ||
                    x.Status == KYCStatus.UnderReview)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkUnderReviewAsync(int kycId, int adminId)
        {
            var doc = await GetPendingAsync(kycId);
            if (doc == null)
                throw new InvalidOperationException("KYC not found or already processed");

            doc.Status = KYCStatus.UnderReview;
            doc.ReviewedByAdminId = adminId;

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                AuditAction.AdminActionPerformed,
                "KYCDocument",
                kycId,
                doc.CustomerId,
                $"KYC marked under review by ID: {adminId}"
            );
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

            await _auditService.LogAsync(
                AuditAction.AdminActionPerformed,
                "KYCDocument",
                kycId,
                doc.CustomerId,
                $"KYC approved by ID: {adminId}"
            );
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

            await _auditService.LogAsync(
                AuditAction.AdminActionPerformed,
                "KYCDocument",
                kycId,
                doc.CustomerId,
                $"KYC rejected by ID: {adminId} - Reason: {reviewNotes}"
            );
        }
    }
}
