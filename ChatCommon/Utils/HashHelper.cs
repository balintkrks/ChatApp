using System.Security.Cryptography;
using System.Text;

namespace ChatCommon.Utils
{
    public static class HashHelper
    {
        public static string Sha256(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }
    }
}
