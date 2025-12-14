namespace BankingSystemAPI.Security.Interfaces
{
    public interface IPasswordHasher
    {
        void CreateHash(string password, out string hash, out string salt);
        bool Verify(string password, string storedHash, string storedSalt);
    }
}
