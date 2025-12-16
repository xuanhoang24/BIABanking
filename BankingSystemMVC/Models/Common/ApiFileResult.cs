namespace BankingSystemMVC.Models.Common
{
    public class ApiFileResult
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = "application/octet-stream";
    }
}
