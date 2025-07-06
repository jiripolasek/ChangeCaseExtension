// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Runtime.CompilerServices;

namespace JPSoftworks.ChangeCaseExtension.Helpers;

internal static class MarkdownHelpers
{
    // https://github.com/xoofx/markdig/blob/7ff8db9016593b71f9ae17d9b2b053fbd54e9cdf/src/Markdig/Helpers/CharHelper.cs#L38C5-L39C1
    // List of characters that need the back-slash prefix
    private const string EscapableChars = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
    private static readonly ulong MaskLo;   // chars 0..63
    private static readonly ulong MaskHi;   // chars 64..127

    static MarkdownHelpers()
    {
        foreach (var c in EscapableChars)
        {
            if (c < 64)
            {
                MaskLo |= 1UL << c;
            }
            else if (c < 128)
            {
                MaskHi |= 1UL << (c - 64);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool NeedsEscape(char c)
    {
        if (c < 64)
        {
            return ((MaskLo >> c) & 1UL) != 0;
        }

        if (c < 128)
        {
            return ((MaskHi >> (c - 64)) & 1UL) != 0;
        }

        return false;
    }

    /// <summary>
    /// Escapes every Markdown-special character by prefixing it with a back-slash.
    /// </summary>
    public static string Escape(ReadOnlySpan<char> input)
    {
        // Pass 1 – count how many extra back-slashes we’ll need.
        int extra = 0;
        foreach (char ch in input)
        {
            if (NeedsEscape(ch))
            {
                extra++;
            }
        }

        // Fast-path: no work required.
        if (extra == 0)
        {
            return input.ToString();
        }

        // Pass 2 – build the final string in one go.
        // string.Create() gives us an exactly-sized string with no temp buffers.
        return string.Create(input.Length + extra, input, static (span, src) =>
        {
            int pos = 0;
            foreach (char ch in src)
            {
                if (NeedsEscape(ch))
                {
                    span[pos++] = '\\';
                }

                span[pos++] = ch;
            }
        });
    }


    public static string WrapInCodeBlock(string content, string language = "")
    {
        if (string.IsNullOrEmpty(content))
        {
            return $"```{language}\n\n```";
        }

        // Find the longest sequence of consecutive backticks
        var maxConsecutiveBackticks = GetMaxConsecutiveBackticks(content);

        // Use at least 3, but more than the max found in content
        var fenceLength = Math.Max(3, maxConsecutiveBackticks + 1);
        var fence = new string('`', fenceLength);

        return $"{fence}{language}\n{content}\n{fence}";

        static int GetMaxConsecutiveBackticks(string text)
        {
            var max = 0;
            var current = 0;

            foreach (var c in text)
            {
                if (c == '`')
                {
                    current++;
                    max = Math.Max(max, current);
                }
                else
                {
                    current = 0;
                }
            }

            return max;
        }
    }
}