using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Application.Dtos.Reports
{
    public class CreateReportMessageDto
    {
        [Required, StringLength(1000)]
        public string Message { get; set; } = string.Empty;
    }
}
