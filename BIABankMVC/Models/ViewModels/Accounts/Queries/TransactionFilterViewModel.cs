namespace BankingSystemMVC.Models.ViewModels.Accounts
{
    public class TransactionFilterViewModel
    {
        public int AccountId { get; set; }
        public string? TransactionType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Reference { get; set; }

        public bool HasFilters =>
            !string.IsNullOrEmpty(TransactionType) ||
            FromDate.HasValue ||
            ToDate.HasValue ||
            !string.IsNullOrEmpty(Reference);
    }
}
