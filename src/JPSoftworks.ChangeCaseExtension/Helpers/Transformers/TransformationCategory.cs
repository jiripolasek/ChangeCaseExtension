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
        Strings.TransformationCategory_Text_Title!,
        Strings.TransformationCategory_Text_Description!,
        IsPreservingDiacritics: true,
        IsPreservingCasing: true,
        IsPreservingSeparators: true,
        IsPreservingSpecialCharacters: true,
        LinePreprocessor: null);

    public static readonly TransformationCategory Technical = new(
        Strings.TransformationCategory_Technical_Title!,
        Strings.TransformationCategory_Technical_Description!,
        IsPreservingDiacritics: false,
        IsPreservingCasing: false,
        IsPreservingSeparators: false,
        IsPreservingSpecialCharacters: false,
        LinePreprocessor: WhitespaceLinePreprocessor);

    public static readonly TransformationCategory Separators = new(
        Strings.TransformationCategory_Separators_Title!,
        Strings.TransformationCategory_Separators_Description!,
        IsPreservingDiacritics: true,
        IsPreservingCasing: true,
        IsPreservingSeparators: true,
        IsPreservingSpecialCharacters: false,
        FilePreprocessor: static arg =>
        {
            var r = StringCaseDetector.DetectAndExtractWordsMultiLine(arg);
            return r is { HasInnerWhitespace: false, HasSpecialSeparators: true, MostFrequentSeparator: not null }
                ? arg.Replace(r.MostFrequentSeparator.Value, ' ')
                : arg;
        });

    public static readonly TransformationCategory Cleanup = new(
        Strings.TransformationCategory_Cleanup_Title!,
        Strings.TransformationCategory_Cleanup_Description!,
        IsPreservingDiacritics: false,
        IsPreservingCasing: false,
        IsPreservingSeparators: false,
        IsPreservingSpecialCharacters: true);

    private static string[] WhitespaceLinePreprocessor(string arg)
    {
        arg = StringNormalizer.RemoveDiacritics(arg);
        arg = StringNormalizer.ReplaceSpecialCharacters(arg);
        return arg.Split();
    }
}