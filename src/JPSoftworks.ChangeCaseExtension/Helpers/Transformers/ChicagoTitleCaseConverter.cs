// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Collections.Frozen;
using System.Text.RegularExpressions;

namespace JPSoftworks.ChangeCaseExtension.Helpers.Transformers;

internal static partial class ChicagoTitleCaseConverter
{
    // Using FrozenSet for better performance with immutable collections
    private static readonly FrozenSet<string> Articles
        = new[] { "a", "an", "the" }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    private static readonly FrozenSet<string> CoordinatingConjunctions
        = new[] { "and", "but", "for", "nor", "or", "so", "yet" }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    private static readonly FrozenSet<string> ShortPrepositions = new[]
    {
        "as", "at", "by", "for", "from", "in", "into", "like", "near", "of", "off", "on", "onto", "over", "past",
        "to", "up", "upon", "with"
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    private static readonly FrozenSet<string> LongPrepositions = new[]
    {
        "about", "above", "across", "after", "against", "along", "among", "around", "before", "behind", "below",
        "beneath", "beside", "between", "beyond", "during", "except", "inside", "outside", "through", "toward",
        "under", "underneath", "until", "within", "without"
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    // Using source generators for regex compilation
    [GeneratedRegex(@"(\s+|(?=[^\w])|(?<=[^\w]))", RegexOptions.Compiled)]
    private static partial Regex TokenizerRegex();

    [GeneratedRegex(@"\w", RegexOptions.Compiled)]
    private static partial Regex WordCharRegex();

    /// <summary>
    /// Converts a string to title case following Chicago Manual of Style rules.
    /// </summary>
    /// <param name="input">The string to convert</param>
    /// <returns>The title-cased string</returns>
    public static string ToChicagoTitleCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        // Split the string into tokens while preserving delimiters
        string[] tokens = TokenizerRegex().Split(input);
        var processedTokens = new StringBuilder(input.Length);
        List<int> wordIndices = [];

        // Identify which tokens are actual words
        for (int i = 0; i < tokens.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(tokens[i]) && WordCharRegex().IsMatch(tokens[i]))
            {
                wordIndices.Add(i);
            }
        }

        // Process each token
        for (int index = 0; index < tokens.Length; index++)
        {
            var token = tokens[index];

            // Skip empty tokens or non-word characters
            if (string.IsNullOrWhiteSpace(token) || !WordCharRegex().IsMatch(token))
            {
                processedTokens.Append(token);
                continue;
            }

            // Determine if this is the first or last word using pattern matching
            var position = (wordIndices.Count, i: index) switch
            {
                (> 0, _) when index == wordIndices[0] => WordPosition.First,
                (> 0, _) when index == wordIndices[^1] => WordPosition.Last,
                _ => WordPosition.Middle
            };

            // Check for hyphenated words
            processedTokens.Append(token.Contains('-')
                ? ProcessHyphenatedWord(token, position)
                : ProcessWord(token, position));
        }

        return processedTokens.ToString();
    }

    private static string ProcessWord(string word, WordPosition position) => position switch
    {
        WordPosition.First or WordPosition.Last => CapitalizeWord(word),
        _ when Articles.Contains(word) => word.ToLowerInvariant(),
        _ when CoordinatingConjunctions.Contains(word) => word.ToLowerInvariant(),
        _ when word.Length < 5 && ShortPrepositions.Contains(word) => word.ToLowerInvariant(),
        _ => CapitalizeWord(word)
    };

    private static string ProcessHyphenatedWord(string word, WordPosition position)
    {
        string[] parts = word.Split('-');
        var processedParts = new StringBuilder();

        for (int i = 0; i < parts.Length; i++)
        {
            if (i > 0)
                processedParts.Append('-');

            if (string.IsNullOrWhiteSpace(parts[i]))
                continue;

            // Use pattern matching to determine if we should capitalize
            var shouldCapitalize = (position, i) switch
            {
                (WordPosition.First, 0) => true,
                (WordPosition.Last, _) when i == parts.Length - 1 => true,
                _ => !IsLowercaseWord(parts[i])
            };

            processedParts.Append(shouldCapitalize ? CapitalizeWord(parts[i]) : parts[i].ToLowerInvariant());
        }

        return processedParts.ToString();
    }

    private static bool IsLowercaseWord(string word) =>
        Articles.Contains(word) ||
        CoordinatingConjunctions.Contains(word) ||
        (word.Length < 5 && ShortPrepositions.Contains(word));

    private static string CapitalizeWord(string word) => word switch
    {
        null or "" => word ?? "",
        // Handle all-caps words (like acronyms)
        _ when word.Length > 1 && word.All(char.IsUpper) => word,
        // Handle mixed case (like iPhone, eBay)
        _ when word.Length > 1 && char.IsLower(word[0]) && word.Any(char.IsUpper) => word,
        // Standard capitalization using string interpolation
        _ => $"{char.ToUpperInvariant(word[0])}{word[1..].ToLowerInvariant()}"
    };

    private enum WordPosition
    {
        First,
        Middle,
        Last
    }
}