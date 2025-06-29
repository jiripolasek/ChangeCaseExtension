// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Helpers.Transformers;

internal static class StringNormalizer
{
    public static string RemoveDiacritics(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var normalized = input.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();
        foreach (var c in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string ReplaceSpecialCharacters(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var result = new StringBuilder();

        bool lastWasSpecial = false;
        foreach (var c in input)
        {
            if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
            {
                if (lastWasSpecial)
                {
                    result.Append(' ');
                    lastWasSpecial = false;
                }

                result.Append(c);
            }
            else
            {
                if (!lastWasSpecial)
                {
                    lastWasSpecial = true;
                }
            }
        }

        if (result.Length > 0 && result[^1] == ' ')
        {
            result.Length--; // remove trailing space
        }

        return result.ToString();
    }

    public static string RemoveSpecialCharacters(string input)
    {
        return new string([.. input.Where(static c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))]);
    }


    /// <summary>
    /// High-performance version using StringBuilder for very large texts
    /// </summary>
    public static string RemoveDuplicateWhitespacePerformance(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var sb = new StringBuilder(input.Length);
        int i = 0;

        while (i < input.Length)
        {
            if (char.IsWhiteSpace(input[i]))
            {
                // Collect all consecutive whitespace
                int start = i;
                int lineBreakCount = 0;
                bool hasTab = false;
                bool hasSpace = false;

                while (i < input.Length && char.IsWhiteSpace(input[i]))
                {
                    if (input[i] == '\n' || input[i] == '\r')
                    {
                        // Handle \r\n as a single line break
                        if (input[i] == '\r' && i + 1 < input.Length && input[i + 1] == '\n')
                        {
                            i++;
                        }

                        lineBreakCount++;
                    }
                    else if (input[i] == '\t')
                    {
                        hasTab = true;
                    }
                    else if (input[i] == ' ')
                    {
                        hasSpace = true;
                    }

                    i++;
                }

                // Append the highest priority whitespace
                if (lineBreakCount > 0)
                {
                    // Maximum 2 line breaks (1 empty line)
                    int breaksToAdd = Math.Min(lineBreakCount, 2);
                    for (int j = 0; j < breaksToAdd; j++)
                    {
                        sb.Append(Environment.NewLine);
                    }
                }
                else if (hasTab)
                {
                    sb.Append('\t');
                }
                else if (hasSpace)
                {
                    sb.Append(' ');
                }
            }
            else
            {
                sb.Append(input[i]);
                i++;
            }
        }

        return sb.ToString();
    }
}