// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Helpers;

public static class StringCaseDetector
{
    private static readonly char[] Separators = ['_', '-', '.', '/', '\\'];

    public static (bool hasSpecialSeparators, string[] words) DetectAndExtractWords(string input)
    {
        if (string.IsNullOrEmpty(input))
            return (false, []);

        // Check if string contains whitespace - if yes, it's not a special case
        if (input.Any(char.IsWhiteSpace))
            return (false, [input.Trim()]);

        // Find the most frequent separator
        var (hasSeparator, mostFrequentSeparator) = FindMostFrequentSeparator(input);

        // Check for camelCase/PascalCase (no separators but mixed case)
        var hasMixedCase = HasMixedCase(input);

        // If no separators and no mixed case, it's just a single word
        if (!hasSeparator && !hasMixedCase)
            return (false, [input]);

        // Extract words using only the most frequent separator
        var words = ExtractWords(input, mostFrequentSeparator);

        // Validate that we found multiple words
        if (words.Length <= 1)
            return (false, [input]);

        return (true, words);
    }

    private static (bool hasSeparator, char? separator) FindMostFrequentSeparator(string input)
    {
        var separatorCounts = new Dictionary<char, int>();

        // Count occurrences of each separator
        foreach (var separator in Separators)
        {
            var count = input.Count(c => c == separator);
            if (count > 0)
            {
                separatorCounts[separator] = count;
            }
        }

        if (separatorCounts.Count == 0)
            return (false, null);

        // Find the most frequent separator
        // In case of tie, use the priority order (first in Separators array wins)
        var maxCount = separatorCounts.Values.Max();
        var mostFrequent = Separators.First(sep =>
            separatorCounts.ContainsKey(sep) && separatorCounts[sep] == maxCount);

        return (true, mostFrequent);
    }

    private static bool HasMixedCase(string input)
    {
        if (input.Length < 2)
            return false;

        var hasUpper = false;
        var hasLower = false;

        foreach (var c in input)
        {
            if (char.IsUpper(c))
                hasUpper = true;
            if (char.IsLower(c))
                hasLower = true;

            if (hasUpper && hasLower)
                return true;
        }

        return false;
    }

    private static string[] ExtractWords(string input, char? separator)
    {
        var words = new List<string>();

        if (separator.HasValue)
        {
            // Split by the chosen separator only
            var parts = input.Split(separator.Value, StringSplitOptions.RemoveEmptyEntries);

            // For each part, check if it contains camelCase/PascalCase
            foreach (var part in parts)
            {
                if (HasMixedCase(part))
                {
                    // Split camelCase/PascalCase
                    words.AddRange(SplitCamelCase(part));
                }
                else if (!string.IsNullOrEmpty(part))
                {
                    words.Add(part);
                }
            }
        }
        else
        {
            // No separators, must be camelCase/PascalCase
            words.AddRange(SplitCamelCase(input));
        }

        return words.ToArray();
    }

    private static string[] SplitCamelCase(string input)
    {
        var words = new List<string>();
        var currentWord = new System.Text.StringBuilder();

        for (var i = 0; i < input.Length; i++)
        {
            var currentChar = input[i];

            if (i == 0)
            {
                currentWord.Append(currentChar);
                continue;
            }

            // Check if we should start a new word
            var shouldSplit = false;

            if (char.IsUpper(currentChar))
            {
                // Check if previous char is lowercase (camelCase boundary)
                if (i > 0 && char.IsLower(input[i - 1]))
                {
                    shouldSplit = true;
                }
                // Check if this is the start of a new word after an acronym
                // e.g., "XMLParser" -> "XML" and "Parser"
                else if (i < input.Length - 1 && char.IsLower(input[i + 1]) && currentWord.Length > 0)
                {
                    shouldSplit = true;
                }
            }

            if (shouldSplit)
            {
                if (currentWord.Length > 0)
                {
                    words.Add(currentWord.ToString());
                    currentWord.Clear();
                }
            }

            currentWord.Append(currentChar);
        }

        if (currentWord.Length > 0)
        {
            words.Add(currentWord.ToString());
        }

        return words.ToArray();
    }

    // Helper method to get separator statistics
    public static Dictionary<char, int> GetSeparatorStatistics(string input)
    {
        var stats = new Dictionary<char, int>();

        foreach (var separator in Separators)
        {
            var count = input.Count(c => c == separator);
            if (count > 0)
            {
                stats[separator] = count;
            }
        }

        return stats;
    }

    // Multi-line version that calculates statistics once but processes each line
    public static MultiLineResult DetectAndExtractWordsMultiLine(string input)
    {
        var result = new MultiLineResult();

        if (string.IsNullOrEmpty(input))
        {
            result.HasSpecialSeparators = false;
            return result;
        }

        // Calculate statistics for the entire string
        result.SeparatorStatistics = GetSeparatorStatistics(input);

        // Find the most frequent separator across all lines
        var (hasSeparator, mostFrequentSeparator) = FindMostFrequentSeparatorFromStats(result.SeparatorStatistics);
        result.HasSpecialSeparators = hasSeparator;
        result.MostFrequentSeparator = mostFrequentSeparator;

        // Check if any line has mixed case (for camelCase detection)
        var anyLineHasMixedCase = false;

        // Split into lines (handle both Windows and Unix line endings)
        var lines = input.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);

        // Process each line individually
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var lineResult = new LineResult { LineNumber = i + 1, OriginalLine = line, HasWords = false, Words = [] };

            // Handle empty lines
            if (string.IsNullOrWhiteSpace(line))
            {
                // For empty lines, return empty array
                result.Lines.Add(lineResult);
                continue;
            }

            // Handle lines that contain spaces
            var trimmedLine = line.Trim();
            if (trimmedLine.Any(static c => char.IsWhiteSpace(c)))
            {
                lineResult.HasWords = true;
                lineResult.Words = trimmedLine.Split();
                lineResult.HasInnerWhitespace = true;
                result.Lines.Add(lineResult);
                result.HasInnerWhitespace = true;
                continue;
            }

            // Check if this line has mixed case
            var lineMixedCase = HasMixedCase(line);
            if (lineMixedCase)
                anyLineHasMixedCase = true;

            // Extract words using the globally determined separator
            var words = ExtractWords(line, mostFrequentSeparator);

            if (words.Length > 1)
            {
                lineResult.HasWords = true;
                lineResult.Words = words;
            }
            else if (words.Length == 1 && lineMixedCase)
            {
                // Single "word" but has mixed case, so it might be camelCase
                var camelWords = SplitCamelCase(words[0]);
                if (camelWords.Length > 1)
                {
                    lineResult.HasWords = true;
                    lineResult.Words = camelWords;
                }
                else
                {
                    // Still camelCase but couldn't be split, return as single element
                    lineResult.HasWords = true;
                    lineResult.Words = [line.Trim()];
                }
            }
            else
            {
                // No special format detected, return the line as a single element
                lineResult.HasWords = true;
                lineResult.Words = [line.Trim()];
            }

            result.Lines.Add(lineResult);
        }

        // Update the overall result based on findings
        if (!result.HasSpecialSeparators && anyLineHasMixedCase)
        {
            result.HasSpecialSeparators = result.Lines.Any(l => l.HasWords);
        }

        return result;
    }

    private static (bool hasSeparator, char? separator) FindMostFrequentSeparatorFromStats(Dictionary<char, int> stats)
    {
        if (stats.Count == 0)
            return (false, null);

        // Find the most frequent separator
        var maxCount = stats.Values.Max();
        var mostFrequent = Separators.First(sep =>
            stats.ContainsKey(sep) && stats[sep] == maxCount);

        return (true, mostFrequent);
    }

    // Result class for multi-line processing
    public class MultiLineResult
    {
        public bool HasSpecialSeparators { get; set; }
        public char? MostFrequentSeparator { get; set; }
        public Dictionary<char, int> SeparatorStatistics { get; set; }
        public List<LineResult> Lines { get; set; } = [];
        public bool HasInnerWhitespace { get; set; }
    }

    public class LineResult
    {
        public int LineNumber { get; set; }
        public string OriginalLine { get; set; }
        public bool HasWords { get; set; } // True if line has content (not empty)
        public string[] Words { get; set; } // Always contains words (unless line is empty)
        public bool WasParsed => this.Words != null && this.Words.Length > 1; // True if special format was detected
        public bool HasInnerWhitespace { get; set; }
    }
}