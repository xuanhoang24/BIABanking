namespace BankingSystemMVC.Areas.Admin.Models.ViewModels.Dashboard
{
    public class DashboardStatsViewModel
    {
        public int TotalCustomers { get; set; }
        public int ActiveAccounts { get; set; }
        public int PendingKYC { get; set; }
        public decimal TodayVolume { get; set; }
    }
}
