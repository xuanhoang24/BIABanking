namespace BankingSystemMVC.Areas.Admin.Models
{
    public class KycFileResult
    {
        public byte[] Bytes { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
}
