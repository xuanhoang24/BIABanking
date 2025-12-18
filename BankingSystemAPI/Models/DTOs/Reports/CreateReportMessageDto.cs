using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Models.DTOs.Reports
{
    public class CreateReportMessageDto
    {
        [Required, StringLength(1000)]
        public string Message { get; set; } = string.Empty;
    }
}
