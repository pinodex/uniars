using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Uniars.Shared.Foundation
{
    public class Helper
    {
        public static readonly char[] alphanumericChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        /// <summary>
        /// Generate 8 character random alphanumeric string.
        /// Warning: this is not cryptographically secure.
        /// </summary>
        /// <returns>Generated string</returns>
        public static string GetRandomAlphaNumeric()
        {
            return Path.GetRandomFileName().Replace(".", "").Substring(0, 8);
        }

        /// <summary>
        /// Generate secure random alphanumeric string.
        /// Props: http://stackoverflow.com/a/1344255/2680698
        /// </summary>
        /// <param name="maxLength">Maximum length of string to be generated. The more the merrier.</param>
        /// <returns>Generated string</returns>
        public static string GetSecureRandomAlphaNumeric(int maxLength = 8)
        {
            byte[] data = new byte[1];

            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);

                data = new byte[maxLength];

                crypto.GetNonZeroBytes(data);
            }

            StringBuilder result = new StringBuilder(maxLength);

            foreach (byte b in data)
            {
                result.Append(alphanumericChars[b % (alphanumericChars.Length)]);
            }

            return result.ToString();
        }
    }
}
