using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace backend_dotnet.Services
{
    public class Hash
    {
        private const int SaltSize = 128 / 8;
        private const int KeySize = 256 / 8;
        private const int Iterations = 10000;
        private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;
        private static char Delimiter = ';';
        // public string HashPassword(string pass) {
        //     var hash = SHA1.Create();
        //     var encoding = new ASCIIEncoding();
        //     var arr = encoding.GetBytes(pass);

        //     var sb = new StringBuilder();
        //     foreach(var character in arr) {
        //         sb.Append(character.ToString("X2"));
        //     }

        //     return sb.ToString();
        // }

        // public bool ValidatePass(string clientPass, string registeredPass) {
        //     var registered = registeredPass.Trim();
        //     var pass = HashPassword(clientPass);
        //     return pass == registered;
        // }
        public string HashPassword(string pass) {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(pass, salt, Iterations, _hashAlgorithmName, KeySize);

            return string.Join(Delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        public bool ValidatePass(string clientPass, string registeredPass) {
            var registered = registeredPass.Trim();
            var elements = registered.Split(Delimiter);
            var salt = Convert.FromBase64String(elements[0]);
            var hash = Convert.FromBase64String(elements[1]);

            var hashInput = Rfc2898DeriveBytes.Pbkdf2(clientPass, salt,Iterations, _hashAlgorithmName,KeySize);

            return CryptographicOperations.FixedTimeEquals(hash,hashInput);
        }
    }
}