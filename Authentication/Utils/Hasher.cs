using System.Security.Cryptography;
using System.Text;

namespace Authentication.Utils
{
    public class Hasher
    {

        public static byte[] ComputeHash(string input)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
            {
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            }
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in ComputeHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

    }
}
