using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace TheFlightShop.Auth
{
    public class Hash : IHash
    {
        private const int DEFAULT_SALT_SIZE = 16;
        private const int DEFAULT_KEY_SIZE = 32;
        private const int DEFAULT_ITERATIONS = 10000;

        public int SaltSize { get; }
        public int KeySize { get; }
        public int Iterations { get; }

        public Hash(int saltSize = DEFAULT_SALT_SIZE, int keySize = DEFAULT_KEY_SIZE, int iterations = DEFAULT_ITERATIONS)
        {
            SaltSize = saltSize;
            KeySize = keySize;
            Iterations = iterations;
        }

        public Tuple<string, string> GenerateHashAndSalt(string value)
        {
            using (var hashingAlgorithm = new Rfc2898DeriveBytes(
                value,
                SaltSize,
                Iterations,
                HashAlgorithmName.SHA512
                )) 
            {
                var hash = Convert.ToBase64String(hashingAlgorithm.GetBytes(KeySize));
                var salt = Convert.ToBase64String(hashingAlgorithm.Salt);
                return new Tuple<string, string>(hash, salt);
            }
        }

        public bool ValueMatches(string value, string hash, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var matches = false;
            using (var hashingAlgorithm = new Rfc2898DeriveBytes(
                value,
                saltBytes,
                Iterations,
                HashAlgorithmName.SHA512
                ))
            {
                var valueBytes = hashingAlgorithm.GetBytes(KeySize);
                var hashBytes = Convert.FromBase64String(hash);
                matches = valueBytes.SequenceEqual(hashBytes);
            }

            return matches;
        }
    }
}
