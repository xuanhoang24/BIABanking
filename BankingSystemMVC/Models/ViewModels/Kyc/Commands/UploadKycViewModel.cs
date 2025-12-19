namespace BankingSystemMVC.Models.ViewModels.Kyc
{
    public class UploadKycViewModel
    {
        public int DocumentType { get; set; }

        public IFormFile File { get; set; } = null!;
    }
}