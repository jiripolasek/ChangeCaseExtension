// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Globalization;

namespace JPSoftworks.ChangeCaseExtension.Helpers.Transformers;

internal static class CaseTransformers
{
    /*
     * camelCase
       Capital Case
       CONSTANT_CASE (aka MACRO_CASE)
       dot.case
       Header-Case (aka Train-Case)
       lower case
       lower First
       no case
       kebab-case (aka param-case)
       KEBAB-UPPER-CASE
       PascalCase
       Pascal_Snake_Case
       path/case
       rAndOm cAsE (aka sPonGE cAsE)
       Sentence case
       snake_case
       sWAP cASE
       Title Case
       UPPER CASE
       Upper first
     */

    private static string[] GetWords(string input)
    {
        return string.IsNullOrWhiteSpace(input)
            ? []
            : input.Split();
    }

    internal static string ToCamelCase(string input)
    {
        var words = GetWords(input);
        if (words.Length == 0) return input;
        return string.Concat(words.Select(static (word, index) =>
            index == 0
                ? word.ToLowerInvariant()
                : char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
    }

    internal static string ToTitleCase(string input)
    {
        return string.IsNullOrWhiteSpace(input)
            ? input
            : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
    }

    internal static string ToTitleCaseInvariant(string input)
    {
        return string.IsNullOrWhiteSpace(input)
            ? input
            : CultureInfo.InvariantCulture.TextInfo.ToTitleCase(input);
    }

    internal static string ToCapitalCase(string input)
    {
        var words = GetWords(input);
        if (words.Length == 0) return input;
        return string.Concat(words.Select(static word =>
            char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
    }

    internal static string ToConstantCase(string input)
    {
        var words = GetWords(input);
        if (words.Length == 0) return input;
        return string.Join("_", words.Select(static word => word.ToUpperInvariant()));
    }

    internal static string ToDotCase(string input)
    {
        var words = GetWords(input);
        if (words.Length == 0) return input;
        return string.Join(".", words.Select(static word => word.ToLowerInvariant()));
    }

    internal static string ToHeaderCase(string input)
    {
        var words = GetWords(input);
        if (words.Length == 0) return input;
        return string.Join("-",
            words.Select(static word => char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
    }

    internal static string ToLowerCase(string input)
    {
        return input.ToLowerInvariant();
    }

    internal static string ToLowerFirst(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        return char.ToLowerInvariant(input[0]) + input[1..];
    }

    internal static string ToNoCase(string input)
    {
        return input; // No transformation, just return the original input
    }

    internal static string ToKebabCase(string input)
    {
        var words = GetWords(input);
        if (words.Length == 0) return input;
        return string.Join("-", words.Select(static word => word.ToLowerInvariant()));
    }

    internal static string ToKebabUpperCase(string input)
    {
        var words = GetWords(input);
        if (words.Length == 0) return input;
        return string.Join("-", words.Select(static word => word.ToUpperInvariant()));
    }

    internal static string ToPascalCase(string input)
    {
        var words = GetWords(input);
        if (words.Length == 0) return input;
        return string.Concat(words.Select(static word =>
            char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
    }

    internal static string ToPascalSnakeCase(string input)
    {
        var words = GetWords(input);
        if (words.Length == 0) return input;
        return string.Join("_",
            words.Select(static word => char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
    }

    internal static string ToPathCase(string input)
    {
        var words = GetWords(input);
        if (words.Length == 0) return input;
        return string.Join("/", words.Select(static word => word.ToLowerInvariant()));
    }

    internal static string ToPathBackslashCase(string input)
    {
        var words = GetWords(input);
        if (words.Length == 0) return input;
        return string.Join("\\", words.Select(static word => word.ToLowerInvariant()));
    }

    internal static string ToRandomCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        var random = new Random();
        return string.Concat(
            input.Select(c => random.Next(2) == 0 ? char.ToLowerInvariant(c) : char.ToUpperInvariant(c)));
    }

    internal static string ToSentenceCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        var words = GetWords(input);
        if (words.Length == 0) return input;

        return string.Join(" ", words.Select(static (word, index) =>
            index == 0
                ? char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()
                : word.ToLowerInvariant()));
    }

    internal static string ToSnakeCase(string input)
    {
        var words = GetWords(input);
        if (words.Length == 0) return input;
        return string.Join("_", words.Select(static word => word.ToLowerInvariant()));
    }

    internal static string ToSwapCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        return string.Concat(input.Select(static c =>
            char.IsUpper(c) ? char.ToLowerInvariant(c) : char.ToUpperInvariant(c)));
    }

    internal static string ToUpperCase(string input)
    {
        return input.ToUpperInvariant();
    }

    internal static string ToUpperFirst(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        return char.ToUpperInvariant(input[0]) + input[1..];
    }

    internal static string ToUpperSnakeCase(string input)
    {
        var words = GetWords(input);
        if (words.Length == 0) return input;
        return string.Join("_", words.Select(static word => word.ToUpperInvariant()));
    }

    internal static string ToUpperFirstSnakeCase(string input)
    {
        var words = GetWords(input);
        if (words.Length == 0) return input;
        return string.Join("_",
            words.Select(static word => char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
    }

    internal static string ToUpperFirstKebabCase(string input)
    {
        var words = GetWords(input);
        if (words.Length == 0) return input;
        return string.Join("-",
            words.Select(static word => char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
    }
}