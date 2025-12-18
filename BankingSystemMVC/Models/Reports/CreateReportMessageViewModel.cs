using System.ComponentModel.DataAnnotations;

namespace BankingSystemMVC.Models.Reports
{
    public class CreateReportMessageViewModel
    {
        [Required, StringLength(1000)]
        public string Message { get; set; } = string.Empty;
    }
}
