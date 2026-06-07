using System.Text;
using System.Security.Cryptography;

namespace Emaktab.Service
{
    public static class HashService
    {
        public static string HashPassword(string password)
        {
            using var md5 = MD5.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = md5.ComputeHash(bytes);
            return Convert.ToHexString(hash); 
        }
    }
}