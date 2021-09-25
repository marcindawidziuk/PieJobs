using System;
using System.Security.Cryptography;

namespace PieJobs.Services
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool IsValid(string password, string hashedPassword);
    }
    
    public class PasswordService : IPasswordService
    {
        private const char Delimiter = '|';
        /// Return a string delimited with random salt, #iterations and hashed password
        /// can be store in database for validation.
        public string HashPassword(string password)
        {
            //generate a random salt for hashing
            var salt = new byte[24];
            new RNGCryptoServiceProvider().GetBytes(salt);

            var iterations = 10000;
            
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            var hash = pbkdf2.GetBytes(24);
            
            // return delimited string with salt | iterations | hash
            return Convert.ToBase64String(salt) + Delimiter + iterations + Delimiter +
                   Convert.ToBase64String(hash);
            
        }

        /// Returns true of hash of test password matches hashed password within origDelimHash
        public bool IsValid(string testPassword, string hashedPassword)
        {
            //extract original values from delimited hash text
            var origHashedParts = hashedPassword.Split(Delimiter);
            var salt = Convert.FromBase64String(origHashedParts[0]);
            var iterations = int.Parse(origHashedParts[1]);
            var origHash = origHashedParts[2];

            //generate hash from test password and original salt and iterations
            var pbkdf2 = new Rfc2898DeriveBytes(testPassword, salt, iterations);
            var testHash = pbkdf2.GetBytes(24);
            
            return Convert.ToBase64String(testHash) == origHash;
        }
    }
}