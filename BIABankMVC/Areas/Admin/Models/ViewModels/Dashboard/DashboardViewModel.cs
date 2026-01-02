using BankingSystemMVC.Areas.Admin.Models.ViewModels.Audit;

namespace BankingSystemMVC.Areas.Admin.Models.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public DashboardStatsViewModel Stats { get; set; } = new();
        public List<AuditLogViewModel> RecentAuditLogs { get; set; } = new();
        public SystemStatusViewModel? SystemStatus { get; set; }
    }
}