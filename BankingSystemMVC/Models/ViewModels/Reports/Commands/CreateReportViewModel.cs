using System.ComponentModel.DataAnnotations;

namespace BankingSystemMVC.Models.ViewModels.Reports
{
    public class CreateReportViewModel
    {
        [Required, StringLength(100)]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(200)]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(2000)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;
    }
}
