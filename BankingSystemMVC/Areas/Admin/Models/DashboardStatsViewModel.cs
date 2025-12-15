namespace BankingSystemMVC.Areas.Admin.Models
{
    public class DashboardStatsViewModel
    {
        public int TotalCustomers { get; set; }
        public int ActiveAccounts { get; set; }
        public int PendingKYC { get; set; }
        public int TodaysTransactions { get; set; }
    }
}
