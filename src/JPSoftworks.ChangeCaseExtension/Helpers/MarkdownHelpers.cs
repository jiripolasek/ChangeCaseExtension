// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Helpers;

internal static class MarkdownHelpers
{
    public static string WrapInCodeBlock(string content, string language = "")
    {
        if (string.IsNullOrEmpty(content))
            return $"```{language}\n\n```";

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