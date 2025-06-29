// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Helpers.Transformers;

internal static class CaseTransformersOnArray
{
    internal static string ToCamelCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        return string.Concat(words.Select(static (word, index) =>
            index == 0
                ? word.ToLowerInvariant()
                : char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
    }

    internal static string ToTitleCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        var joined = string.Join(" ", words);
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(joined);
    }

    internal static string ToTitleCaseInvariant(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        var joined = string.Join(" ", words);
        return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(joined);
    }

    internal static string ToCapitalCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        return string.Join(" ", words.Select(static word =>
            char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
    }

    internal static string ToConstantCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        return string.Join("_", words.Select(static word => word.ToUpperInvariant()));
    }

    internal static string ToDotCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        return string.Join(".", words.Select(static word => word.ToLowerInvariant()));
    }

    internal static string ToHeaderCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        return string.Join("-",
            words.Select(static word => char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
    }

    internal static string ToLowerCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        return string.Join(" ", words).ToLowerInvariant();
    }

    internal static string ToLowerFirst(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        var joined = string.Join(" ", words);
        return char.ToLowerInvariant(joined[0]) + joined[1..];
    }

    internal static string ToNoCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        return string.Join(" ", words); // No transformation, just join the words
    }

    internal static string ToKebabCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        return string.Join("-", words.Select(static word => word.ToLowerInvariant()));
    }

    internal static string ToKebabUpperCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        return string.Join("-", words.Select(static word => word.ToUpperInvariant()));
    }

    internal static string ToPascalCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        return string.Concat(words.Select(static word =>
            char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
    }

    internal static string ToPascalSnakeCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        return string.Join("_",
            words.Select(static word => char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
    }

    internal static string ToPathCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        return string.Join("/", words.Select(static word => word.ToLowerInvariant()));
    }

    internal static string ToPathBackslashCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        return string.Join("\\", words.Select(static word => word.ToLowerInvariant()));
    }

    internal static string ToSpaceCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        return string.Join(" ", words.Select(static word => word.ToLowerInvariant()));
    }

    internal static string ToRandomCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        var random = new Random();
        var joined = string.Join(" ", words);
        return string.Concat(
            joined.Select(c => random.Next(2) == 0 ? char.ToLowerInvariant(c) : char.ToUpperInvariant(c)));
    }

    internal static string ToSentenceCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        return string.Join(" ", words.Select(static (word, index) =>
            index == 0 && word.Length > 0
                ? char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()
                : word.ToLowerInvariant()));
    }

    internal static string ToSnakeCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        return string.Join("_", words.Select(static word => word.ToLowerInvariant()));
    }

    internal static string ToSwapCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        var joined = string.Join(" ", words);
        return string.Concat(joined.Select(static c =>
            char.IsUpper(c) ? char.ToLowerInvariant(c) : char.ToUpperInvariant(c)));
    }

    internal static string ToUpperCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        return string.Join(" ", words).ToUpperInvariant();
    }

    internal static string ToUpperFirst(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        var joined = string.Join(" ", words);
        return char.ToUpperInvariant(joined[0]) + joined[1..];
    }

    internal static string ToUpperSnakeCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        return string.Join("_", words.Select(static word => word.ToUpperInvariant()));
    }

    internal static string ToUpperFirstSnakeCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        return string.Join("_",
            words.Select(static word => char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
    }

    internal static string ToUpperFirstKebabCase(string[] words)
    {
        if (words == null || words.Length == 0) return string.Empty;
        return string.Join("-",
            words.Select(static word => char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
    }
}