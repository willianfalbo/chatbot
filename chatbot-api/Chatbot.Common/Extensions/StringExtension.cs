using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Dlp.Framework;
using Newtonsoft.Json;

namespace Chatbot.Common.Extensions
{
    public static class StringExtension
    {
        /// <summary>Checks if the current CNPJ is valid.</summary>
        /// <returns>Boolean</returns>
        public static bool IsCnpjValid(this string source) =>
            source.IsValidCnpj();

        /// <summary>Checks if the current CPF is valid.</summary>
        /// <returns>Boolean</returns>
        public static bool IsCpfValid(this string source) =>
            source.IsValidCpf();

        /// <summary>Checks if the current EmailAddress is valid.</summary>
        /// <returns>Boolean</returns>
        public static bool IsEmailAddressValid(this string source) =>
            source.IsValidEmailAddress();

        /// <summary>Replaces all the accented characters with its unaccented versions.</summary>
        /// <returns>Output example: Acentuacao (Acentuação)</returns>
        public static string Unaccented(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;

            var chars = source
                .Normalize(NormalizationForm.FormD)
                .ToCharArray()
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();

            return new string(chars).Normalize(NormalizationForm.FormC);
        }

        private static char[] AllowedChars() =>
            "~!@#$%^&*()_+{}| :\"<>?`-=[]\\;',./".ToArray();

        /// <summary>Get all the digits of the specified string.</summary>
        /// <param name="allowChar">To enable specific range of characters.</param>
        /// <returns>Output example: 18236120000158 (18.236.120/0001-58)</returns>
        public static string Digits(this string source, params char[] allowChars)
        {
            var allowedStrings = new string(allowChars?.Distinct()?.Where(i => AllowedChars().Contains(i))?.ToArray());
            allowedStrings = allowedStrings?.Replace("\\", "\\\\"); // replace reserved character in regex
            return Regex.Replace(source, $"[^\\d{allowedStrings}]", string.Empty);
        }

        /// <summary>Get all the letters of the specified string. It also includes accented characters.</summary>
        /// <param name="allowChar">To enable specific range of characters.</param>
        /// <returns>Output example: Adaptação (Adaptação 51 !@#$%^))</returns>
        public static string Letters(this string source, params char[] allowChars)
        {
            var allowedStrings = new string(allowChars?.Distinct()?.Where(i => AllowedChars().Contains(i))?.ToArray());
            allowedStrings = allowedStrings?.Replace("\\", "\\\\"); // replace reserved character in regex
            return Regex.Replace(source, string.Concat("[^\\p{L}", allowedStrings, "]"), string.Empty);
        }

        /// <summary>Get all the letters and Digits of the specified string.</summary>
        /// <param name="allowChar">To enable specific range of characters.</param>
        /// <returns>Output example: file101review (file:101 review)</returns>
        public static string LettersOrDigits(this string source, params char[] allowChars)
        {
            var allowedStrings = new string(allowChars?.Distinct()?.Where(i => AllowedChars().Contains(i))?.ToArray());
            allowedStrings = allowedStrings?.Replace("\\", "\\\\"); // replace reserved character in regex
            return Regex.Replace(source, string.Concat("[^\\d\\p{L}", allowedStrings, "]"), string.Empty);
        }

        /// <summary>Mask the current creditcard number for public display in an interface.</summary>
        /// <returns>Output example: ************1111 (4111111111111111)</returns>
        public static string MaskCreditCard(this string source) =>
            source.Mask(StringMaskFormat.CreditCard);

        /// <summary>Mask the current creditcard number to be saved in a database.</summary>
        /// <returns>Output example: 4111********1111 (4111111111111111)</returns>
        public static string MaskCreditCardExtended(this string source) =>
            source.Mask(StringMaskFormat.CreditCardExtended);

        /// <summary>Mask the current security code for feedback display.</summary>
        /// <returns>Output example: *** (123)</returns>
        public static string MaskPassword(this string source) =>
            source.Mask(StringMaskFormat.Password);

        /// <summary>Compare two strings and ignore Case Sensitive (CS), Accent Sensitive (AS) or Diacritics.</summary>
        /// <param name="item">The target string to compare.</param>
        /// <returns>true for matched string; otherwise, false.</returns>
        public static bool IsEqual(this string source, string item)
        {
            var compareInfo = Thread.CurrentThread.CurrentUICulture.CompareInfo;
            // ignore case sensitive and accent sensitive (diacritics)
            var options = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;
            return compareInfo.Compare(source, item, options) == 0;
        }

        /// <summary>Convert the string to Title Case.</summary>
        /// <returns>Output example: I Love You So Much (I LOVE YOU SO MUCH / i love you so much)</returns>
        public static string TitleCase(this string source) =>
            string.IsNullOrWhiteSpace(source) ? string.Empty : Thread.CurrentThread.CurrentUICulture.TextInfo.ToTitleCase(source?.Trim()?.ToLower());

        /// <summary>Check if the string has a valid Json Schema.</summary>
        /// <returns>true for valid json schema; otherwise, false.</returns>
        public static bool IsJsonSchema(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return false;

            try
            {
                JsonConvert.DeserializeObject(source);
                return true;
            }
            catch //(Exception ex)
            {
                return false;
            }
        }

        /// <summary>Check if the provided name contains only valid letters accented ones.</summary>
        /// <returns>true for valid json schema; otherwise, false</returns>
        public static bool IsNameValid(this string source) =>
            source.IsEqual(source?.Letters(' '));
    }
}