using BankingSystemAPI.Models.Reports;
using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Models.DTOs.Reports
{
    public class UpdateReportStatusDto
    {
        [Required]
        public ReportStatus Status { get; set; }
    }
}
