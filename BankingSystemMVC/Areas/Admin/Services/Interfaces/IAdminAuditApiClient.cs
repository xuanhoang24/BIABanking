using BankingSystemMVC.Areas.Admin.Models;

namespace BankingSystemMVC.Areas.Admin.Services.Interfaces
{
    public interface IAdminAuditApiClient
    {
        Task<List<AuditLogViewModel>> GetRecentAsync();
        Task<List<AuditLogViewModel>> GetAllAsync(
            int page = 1,
            int pageSize = 50,
            int? actionFilter = null,
            string? entityFilter = null,
            string? userIdFilter = null,
            DateTime? dateFilter = null);
    }
}