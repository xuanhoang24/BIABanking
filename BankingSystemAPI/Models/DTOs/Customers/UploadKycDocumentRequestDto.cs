using BankingSystemAPI.Models.Users.Customers;
using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Models.DTOs.Customer
{
    public class UploadKycDocumentRequestDto
    {
        [Required]
        public DocumentType DocumentType { get; set; }

        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
