namespace BankingSystemMVC.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public DashboardStatsViewModel Stats { get; set; } = new();
        public List<AuditLogViewModel> RecentAuditLogs { get; set; } = new();
    }
}