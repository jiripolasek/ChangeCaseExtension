// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Runtime.CompilerServices;

namespace JPSoftworks.ChangeCaseExtension.Helpers.Transformers;

internal static class StringNormalizer
{
    private const char NonBreakingSpaceChar = '\u00A0';



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

        var lastWasSpecial = false;
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
        return new([.. input.Where(static c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))]);
    }



    /// <summary>
    ///     High-performance version using StringBuilder for very large texts
    /// </summary>
    public static string RemoveDuplicateWhitespacePerformance(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var sb = new StringBuilder(input.Length);
        var i = 0;

        while (i < input.Length)
        {
            if (char.IsWhiteSpace(input[i]))
            {
                // Collect all consecutive whitespace
                var start = i;
                var lineBreakCount = 0;
                var hasTab = false;
                var hasSpace = false;

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
                    var breaksToAdd = Math.Min(lineBreakCount, 2);
                    for (var j = 0; j < breaksToAdd; j++)
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

    /// <summary>
    /// Fast single-pass space de-duplication - collapses consecutive spaces while preserving space type
    /// (regular spaces stay regular, non-breaking spaces stay non-breaking) and preserving all other 
    /// whitespace characters (tabs, newlines, etc.) exactly as-is
    /// </summary>
    public static string RemoveDuplicateSpaces(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var buffer = input.Length <= 1024
            ? stackalloc char[input.Length]
            : new char[input.Length];

        var actualLength = RemoveDuplicateSpaces(input.AsSpan(), buffer);

        return actualLength == input.Length
            ? input
            : new(buffer[..actualLength]);
    }

    /// <summary>
    /// Span-based space de-duplication for zero-allocation scenarios. 
    /// If a sequence contains ANY regular space, outputs regular space (text is breakable anyway).
    /// If a sequence contains ONLY non-breaking spaces, preserves non-breaking space.
    /// </summary>
    public static int RemoveDuplicateSpaces(ReadOnlySpan<char> source, Span<char> destination)
    {
        var writeIndex = 0;
        var lastWasSpace = false;

        for (var i = 0; i < source.Length; i++)
        {
            var current = source[i];

            if (IsSpaceCharacter(current))
            {
                if (!lastWasSpace)
                {
                    // Start of a space sequence - look ahead to determine type
                    var spaceToWrite = DetermineSpaceType(source, i);
                    destination[writeIndex++] = spaceToWrite;
                    lastWasSpace = true;
                }
                // Skip additional spaces in the sequence
            }
            else
            {
                destination[writeIndex++] = current;
                lastWasSpace = false;
            }
        }

        return writeIndex;
    }

    /// <summary>
    /// Determines what type of space to output for a sequence starting at the given position
    /// Returns regular space if ANY space in the sequence is regular, non-breaking space otherwise
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static char DetermineSpaceType(ReadOnlySpan<char> source, int startIndex)
    {
        for (var i = startIndex; i < source.Length && IsSpaceCharacter(source[i]); i++)
        {
            if (source[i] == ' ')
            {
                // Found a regular space - entire sequence should be breakable
                return ' ';
            }
        }

        return NonBreakingSpaceChar;
    }

    /// <summary>
    /// Checks if character is a space that should be de-duplicated
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSpaceCharacter(char c) => c is ' ' or NonBreakingSpaceChar;
}