namespace BankingSystemMVC.Areas.Admin.Models.ViewModels.Reports
{
    public class AdminReportViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public AdminReportStatus Status { get; set; }
        public int CustomerId { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
