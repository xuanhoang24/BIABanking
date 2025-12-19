using BankingSystemAPI.Domain.Entities.Security;
using BankingSystemAPI.Infrastructure.Security.Interfaces;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace BankingSystemAPI.Infrastructure.Security.Implements
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordOptions _options;

        public PasswordHasher(IOptions<PasswordOptions> options)
        {
            _options = options.Value;
        }

        public void CreateHash(string password, out string hash, out string salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                _options.SaltSize,
                _options.Iterations,
                HashAlgorithmName.SHA256
            );

            salt = Convert.ToBase64String(pbkdf2.Salt);
            hash = Convert.ToBase64String(
                pbkdf2.GetBytes(_options.HashSize)
            );
        }

        public bool Verify(string password, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            var storedHashBytes = Convert.FromBase64String(storedHash);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                saltBytes,
                _options.Iterations,
                HashAlgorithmName.SHA256
            );

            var computedHashBytes = pbkdf2.GetBytes(_options.HashSize);

            return CryptographicOperations.FixedTimeEquals(
                storedHashBytes,
                computedHashBytes
            );
        }
    }
}
