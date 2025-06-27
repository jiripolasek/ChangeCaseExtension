// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Globalization;

namespace JPSoftworks.ChangeCaseExtension.Helpers.Transformers;

internal static class TransformationRegistry
{
    private static readonly TransformationDefinition[] TransformationDefinitions =
    [
        new TransformationDefinitionWords(TransformationType.AsIs, "input as is (no change)", TransformationCategory.Text,
            CaseTransformersOnArray.ToNoCase),

        // text
        new TransformationDefinitionWords(TransformationType.LowerCase, "lower case", TransformationCategory.Text,
            CaseTransformersOnArray.ToLowerCase),
        new TransformationDefinitionWords(TransformationType.UpperCase, "UPPER CASE", TransformationCategory.Text,
            CaseTransformersOnArray.ToUpperCase),
        new TransformationDefinitionWords(TransformationType.SentenceCase, "Sentence case", TransformationCategory.Text,
            CaseTransformersOnArray.ToSentenceCase),
        new TransformationDefinitionWords(TransformationType.LowerFirst, "lower First", TransformationCategory.Text,
            CaseTransformersOnArray.ToLowerFirst),
        new TransformationDefinitionWords(TransformationType.UpperFirst, "Upper first", TransformationCategory.Text,
            CaseTransformersOnArray.ToUpperFirst),
        new TransformationDefinitionWords(TransformationType.CapitalCase, "Capital Case", TransformationCategory.Text,
            CaseTransformersOnArray.ToCapitalCase),
        new TransformationDefinitionWords(TransformationType.TitleCase,
            $"Title Case ({CultureInfo.CurrentCulture.DisplayName})", TransformationCategory.Text,
            CaseTransformersOnArray.ToTitleCase),
        new TransformationDefinitionWords(TransformationType.TitleCaseInvariant, "Title Case (invariant)",
            TransformationCategory.Text, CaseTransformersOnArray.ToTitleCaseInvariant),
        new TransformationDefinitionWords(TransformationType.SwapCase, "sWAP cASE", TransformationCategory.Text,
            CaseTransformersOnArray.ToSwapCase),
        new TransformationDefinitionWords(TransformationType.RandomCase, "rAndOm cAsE", TransformationCategory.Text,
            CaseTransformersOnArray.ToRandomCase),

        // technical
        new TransformationDefinitionWords(TransformationType.CamelCase, "camelCase", TransformationCategory.Technical,
            CaseTransformersOnArray.ToCamelCase),
        new TransformationDefinitionWords(TransformationType.PascalCase, "PascalCase", TransformationCategory.Technical,
            CaseTransformersOnArray.ToPascalCase),
        new TransformationDefinitionWords(TransformationType.SnakeCase, "snake_case", TransformationCategory.Technical,
            CaseTransformersOnArray.ToSnakeCase),
        new TransformationDefinitionWords(TransformationType.UpperSnakeCase, "UPPER_SNAKE_CASE",
            TransformationCategory.Technical, CaseTransformersOnArray.ToUpperSnakeCase),
        new TransformationDefinitionWords(TransformationType.PascalSnakeCase, "Pascal_Snake_Case",
            TransformationCategory.Technical, CaseTransformersOnArray.ToPascalSnakeCase),
        new TransformationDefinitionWords(TransformationType.ConstantCase, "CONSTANT_CASE",
            TransformationCategory.Technical, CaseTransformersOnArray.ToConstantCase),
        new TransformationDefinitionWords(TransformationType.KebabCase, "kebab-case", TransformationCategory.Technical,
            CaseTransformersOnArray.ToKebabCase),
        new TransformationDefinitionWords(TransformationType.KebabUpperCase, "KEBAB-UPPER-CASE",
            TransformationCategory.Technical, CaseTransformersOnArray.ToKebabUpperCase),
        new TransformationDefinitionWords(TransformationType.HeaderCase, "Header-Case",
            TransformationCategory.Technical, CaseTransformersOnArray.ToHeaderCase),
        new TransformationDefinitionWords(TransformationType.UpperFirstSnakeCase, "Upper first Snake_Case",
            TransformationCategory.Technical, CaseTransformersOnArray.ToUpperFirstSnakeCase),
        new TransformationDefinitionWords(TransformationType.UpperFirstKebabCase, "Upper first Kebab-Case",
            TransformationCategory.Technical, CaseTransformersOnArray.ToUpperFirstKebabCase),

        // separators
        new TransformationDefinitionWords(TransformationType.SpaceCase, "space case", TransformationCategory.Separators,
            CaseTransformersOnArray.ToSpaceCase),
        new TransformationDefinitionWords(TransformationType.DotCase, "dot.case", TransformationCategory.Separators,
            CaseTransformersOnArray.ToDotCase),
        new TransformationDefinitionWords(TransformationType.PathCase, "path/case", TransformationCategory.Separators,
            CaseTransformersOnArray.ToPathCase),
        new TransformationDefinitionWords(TransformationType.PathBackslashCase, @"path\case\backslash",
            TransformationCategory.Separators, CaseTransformersOnArray.ToPathBackslashCase),

        // special
        new TransformationDefinitionAll(TransformationType.RemoveDiacritics, "Remove diacritics",
            TransformationCategory.Cleanup, StringNormalizer.RemoveDiacritics),
        new TransformationDefinitionAll(TransformationType.RemoveSpecialCharacters, "Remove special characters",
            TransformationCategory.Cleanup, StringNormalizer.RemoveSpecialCharacters),
        new TransformationDefinitionAll(TransformationType.RemoveDuplicateWhitespace, "Remove duplicate spaces",
            TransformationCategory.Cleanup, StringNormalizer.RemoveDuplicateWhitespacePerformance)
    ];

    public static IReadOnlyList<TransformationDefinition> GetTransformations()
    {
        return TransformationDefinitions;
    }
}