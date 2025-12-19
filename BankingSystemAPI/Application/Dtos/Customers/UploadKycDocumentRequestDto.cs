using BankingSystemAPI.Domain.Entities.Users.Customers;
using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Application.Dtos.Customers
{
    public class UploadKycDocumentRequestDto
    {
        [Required]
        public DocumentType DocumentType { get; set; }

        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
