using System.Collections;
using System;
using System.Collections.Generic;

namespace Continuum93.CodeAnalysis
{
    public static class GeneralUtils
    {
        public static List<byte[]> SplitArrayByTerminator(byte[] input)
        {
            List<byte[]> response = new();

            int start = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == 0)
                {
                    byte[] subArray = new byte[i - start];
                    Array.Copy(input, start, subArray, 0, i - start);
                    if (subArray.Length > 0) // Add only non-empty arrays
                    {
                        response.Add(subArray);
                    }
                    start = i + 1;
                }
            }
            byte[] finalSubArray = new byte[input.Length - start];
            Array.Copy(input, start, finalSubArray, 0, input.Length - start);
            if (finalSubArray.Length > 0)
                response.Add(finalSubArray);

            return response;
        }

        public static string GetUntilOrWhole(this string text, string stopAt = ":")
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text[..charLocation].Trim();
                }
            }

            return text.Trim();
        }

        public static string ByteArrayToString(byte[] input)
        {
            return string.Join(",", input);
        }
    }
}
