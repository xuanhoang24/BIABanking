using System.ComponentModel.DataAnnotations;

namespace BankingSystemMVC.Models.ViewModels.Reports
{
    public class CreateReportMessageViewModel
    {
        [Required, StringLength(1000)]
        public string Message { get; set; } = string.Empty;
    }
}
