using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Models.DTOs.Reports
{
    public class CreateReportDto
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(2000)]
        public string Description { get; set; } = string.Empty;
    }
}
