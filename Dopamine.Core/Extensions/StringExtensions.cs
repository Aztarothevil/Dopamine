using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Dopamine.Core.Extensions
{
    public static class StringExtensions
    {
        public static string[] Split(this string source, string separator)
        {
            return source.Split(new string[] { separator }, StringSplitOptions.None);
        }

        public static string Trim(this string source, string stringToRemove)
        {
            return source.Trim(stringToRemove.ToCharArray());
        }

        public static string TrimStart(this string input, string prefixToRemove)
        {
            if (input != null && prefixToRemove != null && input.StartsWith(prefixToRemove))
            {
                return input.Substring(prefixToRemove.Length, input.Length - prefixToRemove.Length);
            }
            else
            {
                return input;
            }
        }

        public static long SafeConvertToLong(this string str)
        {
            long parsedLong = 0;
            Int64.TryParse(str, out parsedLong);
            return parsedLong;
        }

        public static bool IsNumeric(this string str)
        {
            float output;
            return float.TryParse(str, out output);
        }

        public static string ToSafePath(this string path)
        {
            return path != null ? path.ToLower() : path;
        }

        public static string MakeUnique(this string proposedString, IList<string> existingStrings)
        {
            string uniqueString = proposedString;

            int number = 1;

            while (existingStrings.Contains(uniqueString))
            {
                number++;
                uniqueString = proposedString + " (" + number + ")";
            }

            return uniqueString;
        }

        public static string CapitalizeFirstLetterExcludingSigns(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input; // Return empty or null string as is
            }

            // Find the index of the first letter
            int firstLetterIndex = -1;
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsLetter(input[i]))
                {
                    firstLetterIndex = i;
                    break;
                }
            }

            // If no letter is found, return the original string
            if (firstLetterIndex == -1)
            {
                return input;
            }

            // Capitalize the first letter found
            char[] charArray = input.ToCharArray();
            charArray[firstLetterIndex] = char.ToUpper(charArray[firstLetterIndex]);

            return new string(charArray);
        }
    }
}
