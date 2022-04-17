using System;
using System.Text.RegularExpressions;

namespace TeamSketch.Utils
{
    public static class ValidationUtil
    {
        public static bool IsAlphanumeric(string text)
        {
            if (text == null)
            {
                throw new ArgumentException(null, nameof(text));
            }

            text = text.Trim().ToLower();

            return Regex.IsMatch(text, "^\\w+$");
        }
    }
}
