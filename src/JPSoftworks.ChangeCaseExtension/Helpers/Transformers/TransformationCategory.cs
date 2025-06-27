// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Helpers.Transformers;

internal record TransformationCategory(
    string DisplayName,
    string Description,
    bool IsPreservingDiacritics = false,
    bool IsPreservingCasing = false,
    bool IsPreservingSeparators = false,
    bool IsPreservingSpecialCharacters = false,
    Func<string, string[]>? LinePreprocessor = null,
    Func<string, string>? FilePreprocessor = null)
{
    public static readonly TransformationCategory Text = new(
        "Text",
        "Preserves diacritics and linguistic features.",
        IsPreservingDiacritics: true,
        IsPreservingCasing: true,
        IsPreservingSeparators: true,
        IsPreservingSpecialCharacters: true,
        LinePreprocessor: null);

    public static readonly TransformationCategory Technical = new(
        "Technical",
        "Normalizes for code identifiers (removes diacritics, etc.).",
        IsPreservingDiacritics: false,
        IsPreservingCasing: false,
        IsPreservingSeparators: false,
        IsPreservingSpecialCharacters: false,
        LinePreprocessor: Preprocessor);

    public static readonly TransformationCategory Separators = new(
        "Separators",
        "Handles separators but doesn't change case.",
        IsPreservingDiacritics: true,
        IsPreservingCasing: true,
        IsPreservingSeparators: true,
        IsPreservingSpecialCharacters: false,
        FilePreprocessor: static arg =>
        {
            var r = StringCaseDetector.DetectAndExtractWordsMultiLine(arg);
            return r is { HasInnerWhitespace: false, HasSpecialSeparators: true }
                ? arg.Replace(r.MostFrequentSeparator!.Value, ' ')
                : arg;
        });

    public static readonly TransformationCategory Cleanup = new(
        "Special",
        "Custom behavior that may not fit other categories.",
        IsPreservingDiacritics: false,
        IsPreservingCasing: false,
        IsPreservingSeparators: false,
        IsPreservingSpecialCharacters: true);

    private static string[] Preprocessor(string arg)
    {
        arg = StringNormalizer.RemoveDiacritics(arg);
        arg = StringNormalizer.ReplaceSpecialCharacters(arg);
        return arg.Split();
    }
}