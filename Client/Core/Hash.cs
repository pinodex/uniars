namespace Uniars.Client.Core
{
    public class Hash
    {
        /// <summary>
        ///     Make hash for a string
        /// </summary>
        /// <param name="text">String to hash</param>
        /// <returns>string</returns>
        public static string Make(string text)
        {
            return BCrypt.Net.BCrypt.HashPassword(text);
        }

        /// <summary>
        ///     Check if string matches a hash
        /// </summary>
        /// <param name="text">Plain-text string</param>
        /// <param name="hash">Hashed string</param>
        /// <returns>bool</returns>
        public static bool Check(string text, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(text, hash);
        }
    }
}
