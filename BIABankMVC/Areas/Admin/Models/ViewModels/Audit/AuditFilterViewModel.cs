namespace BankingSystemMVC.Areas.Admin.Models.ViewModels.Audit
{
    public class AuditFilterViewModel
    {
        public int? ActionFilter { get; set; }
        public string? EntityFilter { get; set; }
        public string? CustomerIdFilter { get; set; }
        public string? SearchRef { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
