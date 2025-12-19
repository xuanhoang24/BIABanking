namespace BankingSystemMVC.Models.ViewModels.Reports
{
    public class ReportViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ReportStatus Status { get; set; }
        public int CustomerId { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
