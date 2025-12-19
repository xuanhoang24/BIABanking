using BankingSystemMVC.Areas.Admin.Models.ViewModels.Audit;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Audit;

namespace BankingSystemMVC.Areas.Admin.Services.Implementations.Audit
{
    public class AdminAuditApiClient : IAdminAuditApiClient
    {
        private readonly HttpClient _http;

        public AdminAuditApiClient(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("AdminApi");
        }

        public async Task<List<AuditLogViewModel>> GetRecentAsync()
        {
            return await _http.GetFromJsonAsync<List<AuditLogViewModel>>(
                "api/admin/audit/recent"
            ) ?? new();
        }

        public async Task<List<AuditLogViewModel>> GetAllAsync(
            int page = 1,
            int pageSize = 50,
            int? actionFilter = null,
            string? entityFilter = null,
            string? customerIdFilter = null,
            DateTime? dateFilter = null)
        {
            var queryParams = new List<string>
            {
                $"page={page}",
                $"pageSize={pageSize}"
            };

            if (actionFilter.HasValue)
                queryParams.Add($"actionFilter={actionFilter}");

            if (!string.IsNullOrEmpty(entityFilter))
                queryParams.Add($"entityFilter={entityFilter}");

            if (!string.IsNullOrEmpty(customerIdFilter))
                queryParams.Add($"customerIdFilter={customerIdFilter}");

            if (dateFilter.HasValue)
                queryParams.Add($"dateFilter={dateFilter:yyyy-MM-dd}");

            var queryString = string.Join("&", queryParams);
            var url = $"api/admin/audit/all?{queryString}";

            return await _http.GetFromJsonAsync<List<AuditLogViewModel>>(url) ?? new();
        }
    }
}
