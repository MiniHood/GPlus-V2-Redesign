using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace GPlus.Source.General
{
    internal static class FileProtection
    {
        public static string Protect(string plainText)
        {
            var data = Encoding.UTF8.GetBytes(plainText);
            var encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
        }

        public static string Unprotect(string cipherText)
        {
            var data = Convert.FromBase64String(cipherText);
            var decrypted = ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}