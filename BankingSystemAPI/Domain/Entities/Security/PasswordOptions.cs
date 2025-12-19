namespace BankingSystemAPI.Domain.Entities.Security
{
    public class PasswordOptions
    {
        public int SaltSize { get; set; }
        public int HashSize { get; set; }
        public int Iterations { get; set; }
        public int MaxFailedAttempts { get; set; }
        public int LockMinutes { get; set; }
    }
}
