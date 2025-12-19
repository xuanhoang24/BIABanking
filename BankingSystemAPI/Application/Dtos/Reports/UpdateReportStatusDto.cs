using BankingSystemAPI.Domain.Entities.Reports;
using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Application.Dtos.Reports
{
    public class UpdateReportStatusDto
    {
        [Required]
        public ReportStatus Status { get; set; }
    }
}
