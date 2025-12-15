namespace BankingSystemAPI.Models.DTOs.Admin
{
    public class DashboardStatsDto
    {
        public int TotalCustomers { get; set; }
        public int ActiveAccounts { get; set; }
        public int PendingKYC { get; set; }
        public int TodaysTransactions { get; set; }
    }
}
