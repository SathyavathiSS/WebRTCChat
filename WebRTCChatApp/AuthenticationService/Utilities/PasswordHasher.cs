using System;
using System.Security.Cryptography;
using System.Text;

namespace AuthenticationService.Utilities
{
    public class PasswordHasher
    {
        // Method to hash the password using SHA256
        public byte[] HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                return sha256.ComputeHash(inputBytes);
            }
        }

        // Method to generate a salt
        public byte[] GenerateSalt(int size = 16)
        {
            var salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        // Method to hash a password with a salt
        public byte[] HashPasswordWithSalt(string password, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = new byte[salt.Length + passwordBytes.Length];
                Buffer.BlockCopy(salt, 0, saltedPassword, 0, salt.Length);
                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, salt.Length, passwordBytes.Length);

                return sha256.ComputeHash(saltedPassword);
            }
        }

        // Method to verify a password against a stored hash and salt
        public bool VerifyPassword(string inputPassword, byte[] storedHash, byte[] storedSalt)
        {
            byte[] inputHash = HashPasswordWithSalt(inputPassword, storedSalt);
            return inputHash.SequenceEqual(storedHash);
        }
    }
}
