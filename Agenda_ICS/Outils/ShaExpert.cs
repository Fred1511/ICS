using System;
using System.Security.Cryptography;
using System.Text;

namespace NOutils
{
    public class ShaExpert
    {
        public static string ComputeSHA256(string txt)
        {
            using(var sha256 = new SHA256Managed())
            {
                var résultatBrut = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(txt))).ToLower();
                var result = string.Empty;
                for(int i = 0; i < résultatBrut.Length; i++)
                {
                    var c = résultatBrut[i];
                    if (char.IsLetterOrDigit(c))
                    {
                        result += c;
                    }
                }

                return result;
            }
        }
    }
}
