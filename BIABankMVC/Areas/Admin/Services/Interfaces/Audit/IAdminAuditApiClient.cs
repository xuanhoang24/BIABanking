using BankingSystemMVC.Areas.Admin.Models.ViewModels.Audit;

namespace BankingSystemMVC.Areas.Admin.Services.Interfaces.Audit
{
    public interface IAdminAuditApiClient
    {
        Task<List<AuditLogViewModel>> GetRecentAsync();
        Task<List<AuditLogViewModel>> GetAllAsync(
            int page = 1,
            int pageSize = 50,
            int? actionFilter = null,
            string? entityFilter = null,
            string? customerIdFilter = null,
            string? searchRef = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);
    }
}