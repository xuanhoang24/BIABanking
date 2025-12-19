namespace BankingSystemAPI.Application.Dtos.Admin
{
    public class DashboardStatsDto
    {
        public int TotalCustomers { get; set; }
        public int ActiveAccounts { get; set; }
        public int PendingKYC { get; set; }
        public decimal TodayVolume { get; set; }
    }
}
